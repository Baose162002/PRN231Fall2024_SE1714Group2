using BusinessObject.DTO.Response;
using BusinessObject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Service;
using Service.IService;
using BusinessObject.Enum;
using BusinessObject.DTO.Request;

namespace WebApi_EventFlowerExchange.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPayService _payService; // Sử dụng để lưu Payment vào database

        public PaymentController(IPayService payService)
        {
            _payService = payService;
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreatePaymentDTO paymentDTO)
        {
            try
            {
                await _payService.CreatePayment(paymentDTO);
                return Ok("Payment successfully");
            }catch(ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
    }

}
