using BusinessObject.DTO.Response;
using BusinessObject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Service;
using Service.IService;
using BusinessObject.Enum;

namespace WebApi_EventFlowerExchange.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly IPayService _payService; // Sử dụng để lưu Payment vào database

        public PaymentController(PaymentService paymentService, IPayService payService)
        {
            _paymentService = paymentService;
            _payService = payService;
        }

        // Tạo URL thanh toán
        [HttpPost("create-payment")]
        public IActionResult CreatePayment(VnPaymentRequestModel booking)
        {
            var paymentUrl = _paymentService.GeneratePaymentUrl(HttpContext, booking);
            return Ok(new { paymentUrl });
        }

        // Xử lý phản hồi sau khi thanh toán
        [HttpGet("payment-callback")]
        public IActionResult PaymentCallback()
        {
            
            var paymentResponse = _paymentService.PaymentExecute(Request.Query);

            if (!paymentResponse.Success)
            {
                return BadRequest(new { message = "Payment failed", paymentResponse });
            }

            // Lưu thông tin thanh toán thành công vào database
            var payment = new Payment
            {
                PaymentStatus = EnumList.PaymentStatus.Completed,
                AmountPaid = decimal.Parse(paymentResponse.Amount) / 100, 
                PaymentDate = DateTime.Now,
                OrderId = int.Parse(paymentResponse.OrderId)
            };

            _payService.CreatePayment(payment);

            return Ok(new { message = "Payment successful", paymentResponse });
        }
    }

}
