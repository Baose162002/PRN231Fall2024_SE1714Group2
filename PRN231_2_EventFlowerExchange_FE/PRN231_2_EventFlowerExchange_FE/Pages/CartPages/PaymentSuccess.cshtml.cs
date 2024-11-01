using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN231_2_EventFlowerExchange_FE.Service;
using System.Configuration;
using System.Text;
using System.Text.Json;

namespace PRN231_2_EventFlowerExchange_FE.Pages.CartPages
{
    public class PaymentSuccessModel : PageModel
    {
        private readonly PaymentService _paymentService;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;

        public PaymentSuccessModel(PaymentService paymentService, IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _paymentService = paymentService;
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGet(int quantity = 1)
        {
            var customerId = HttpContext.Session.GetString("UserId");
            var token = HttpContext.Session.GetString("JWTToken");
            var flowerId = HttpContext.Session.GetString("FlowerId"); // Chuyển đổi thành chuỗi

            try
            {
                var paymentResponse = _paymentService.PaymentExecute(Request.Query);

                if (paymentResponse.Success)
                {
                    List<CartItemDTO> cartItems = new();

                    // Nếu flowerId có giá trị, xem như thanh toán từ nút "Buy"
                    if (!string.IsNullOrEmpty(flowerId)) // Kiểm tra xem flowerId có rỗng không
                    {
                        // Thêm sản phẩm từ nút "Buy" vào giỏ hàng
                        cartItems.Add(new CartItemDTO
                        {
                            FlowerId = flowerId,
                            Quantity = quantity
                        });
                    }
                    else
                    {
                        // Kiểm tra nếu có giỏ hàng
                        var cartJson = HttpContext.Request.Cookies["cartItems"];
                        if (!string.IsNullOrEmpty(cartJson))
                        {
                            cartItems = JsonSerializer.Deserialize<List<CartItemDTO>>(cartJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        }

                        // Kiểm tra lại nếu danh sách `cartItems` vẫn rỗng
                        if (cartItems == null || !cartItems.Any())
                        {
                            return RedirectToPage("/OrderPages/OrderFailure", new { message = "No items to process in the cart." });
                        }
                    }

                    // Kiểm tra token và customerId
                    if (token == null || customerId == null)
                    {
                        return RedirectToPage("/OrderPages/OrderFailure", new { message = "Session expired, please log in again." });
                    }

                    // Lấy địa chỉ khách hàng từ API
                    // Chuyển đổi customerId thành int
                    int parsedCustomerId;
                    if (!int.TryParse(customerId, out parsedCustomerId))
                    {
                        return RedirectToPage("/OrderPages/OrderFailure", new { message = "Invalid customer ID." });
                    }

                    var userApiUrl = $"http://localhost:5077/odata/User?$filter=UserId eq {parsedCustomerId}"; // Sử dụng parsedCustomerId


                    string address = string.Empty;

                    using (var client = _clientFactory.CreateClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        var userResponse = await client.GetAsync(userApiUrl);

                        if (!userResponse.IsSuccessStatusCode)
                        {
                            var errorContent = await userResponse.Content.ReadAsStringAsync();
                            return RedirectToPage("/OrderPages/OrderFailure", new { message = $"Failed to fetch user data: {errorContent}" });
                        }

                        var responseContent = await userResponse.Content.ReadAsStringAsync();
                        var oDataResponse = JsonSerializer.Deserialize<ODataResponse<ListUserDTO>>(responseContent);

                        if (oDataResponse == null || !oDataResponse.Value.Any())
                        {
                            return RedirectToPage("/OrderPages/OrderFailure", new { message = "Failed to retrieve user address." });
                        }

                        address = oDataResponse.Value.First().Address;
                    }

                    // Tạo yêu cầu đơn hàng từ `cartItems`
                    var orderRequest = new
                    {
                        orderDate = DateTime.Now,
                        deliveryAddress = address,
                        deliveryDate = DateTime.Now.AddDays(3),
                        customerId = customerId,
                        orderDetails = cartItems.Select(item => new
                        {
                            flowerId = item.FlowerId,
                            quantityOrdered = item.Quantity
                        }).ToList(),
                    };

                    // Gửi yêu cầu tạo đơn hàng đến API
                    var orderApiUrl = "http://localhost:5077/api/Order/order";

                    using (var client = _clientFactory.CreateClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        var content = new StringContent(JsonSerializer.Serialize(orderRequest), Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(orderApiUrl, content);

                        if (!response.IsSuccessStatusCode)
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            return RedirectToPage("/OrderPages/OrderFailure", new { message = $"Failed to create order: {errorContent}" });
                        }

                        // Xóa giỏ hàng nếu thanh toán qua giỏ hàng
                        if (string.IsNullOrEmpty(flowerId))
                        {
                            HttpContext.Response.Cookies.Delete("cartItems");
                        }
                        HttpContext.Response.Cookies.Delete("cartItems");
                        return RedirectToPage("/OrderPages/OrderSuccess");
                    }
                }

                return RedirectToPage("/OrderPages/OrderFailure");
            }
            catch (Exception ex)
            {
                return RedirectToPage("/OrderPages/OrderFailure", new { message = ex.Message });
            }
        }
    }
}
