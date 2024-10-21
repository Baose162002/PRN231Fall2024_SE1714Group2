using BusinessObject.DTO.Response;

namespace PRN231_2_EventFlowerExchange_FE.Service
{
    public class PaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly VnPayLibrary _vnPayLibrary;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentService(IConfiguration configuration, VnPayLibrary vnPayLibrary, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _vnPayLibrary = vnPayLibrary;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GeneratePaymentUrl(HttpContext context, VnPaymentRequestModel booking)
        {
            // Create a unique transaction reference using the current timestamp
            var transactionRef = DateTime.Now.Ticks.ToString();

            // Instantiate VnPayLibrary to handle request data
            var vnpay = new VnPayLibrary();

            // Add request data based on VNPAY's parameters and the configuration
            vnpay.AddRequestData("vnp_Version", _configuration["VNPAY:Version"]);
            vnpay.AddRequestData("vnp_Command", _configuration["VNPAY:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _configuration["VNPAY:TmnCode"]);

            var amountInSmallestUnit = (booking.Amount * 100).ToString();

            // Ensure the amount is in the correct format (multiplying by 100 to remove decimal points)
            vnpay.AddRequestData("vnp_Amount", amountInSmallestUnit);

            // Add the current timestamp as the creation date of the transaction
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));

            // Add currency code, typically VND (Vietnamese Dong)
            vnpay.AddRequestData("vnp_CurrCode", _configuration["VNPAY:CurrCode"]);

            // Add the client's IP address (useful for tracking)
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));

            // Add locale information, specifying the language ("vn" for Vietnamese)
            vnpay.AddRequestData("vnp_Locale", _configuration["VNPAY:Locale"]);

            // Add order information that includes the booking event's ID
            vnpay.AddRequestData("vnp_OrderInfo", "Payment for event: " + booking.OrderId);

            // Add order type, can be "other" or specific to your business needs
            vnpay.AddRequestData("vnp_OrderType", "other");

            // Add the return URL to redirect users after payment
            vnpay.AddRequestData("vnp_ReturnUrl", _configuration["VNPAY:PaymentBackReturnUrl"]);

            // Add the unique transaction reference for this order (must be unique for each request)
            vnpay.AddRequestData("vnp_TxnRef", transactionRef);

            // Generate the payment URL using the VNPAY library
            var paymentUrl = vnpay.CreateRequestUrl(_configuration["VNPAY:BaseUrl"], _configuration["VNPAY:HashSecret"]);

            // Return the payment URL
            return paymentUrl;
        }

        public VnPaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            // Lấy Transaction ID từ response
            var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));

            // Lấy OrderInfo để trích xuất OrderId
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo"); // Đảm bảo khai báo trước khi sử dụng
            var orderIdString = vnp_OrderInfo.Replace("Payment for event: ", ""); // Trích xuất OrderId từ chuỗi OrderInfo

            // Chuyển đổi OrderId sang kiểu long
            var vnp_orderId = Convert.ToInt64(orderIdString);

            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_Amount = vnpay.GetResponseData("vnp_Amount");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _configuration["VnPay:HashSecret"]);
            if (!checkSignature)
            {
                return new VnPaymentResponseModel
                {
                    Success = false
                };
            }
            if (vnp_ResponseCode != "00") // Assuming "00" means success
            {
                return new VnPaymentResponseModel
                {
                    Success = false,
                    OrderDescription = $"Payment failed with response code: {vnp_ResponseCode}"
                };
            }

            // Trả về thông tin thành công, bao gồm cả OrderId đã lấy
            return new VnPaymentResponseModel
            {
                Success = true,
                PaymentMethod = "VnPay",
                OrderDescription = vnp_OrderInfo,
                OrderId = vnp_orderId.ToString(),
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode,
                Amount = vnp_Amount
            };
        }
    }
}
