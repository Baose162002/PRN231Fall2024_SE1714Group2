using AutoMapper;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.Service;

namespace WebApi_EventFlowerExchange.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly IDeliveryService _deliveryService;
        private readonly IMapper _mapper;

        public DeliveryController(IDeliveryService deliveryService, IMapper mapper)
        {
            _deliveryService = deliveryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDeliveries()
        {
            var deliveries = await _deliveryService.GetAllDeliveries();
            return Ok(deliveries);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeliveryById(int id)
        {
            try
            {
                var delivery = await _deliveryService.GetDeliveryById(id);
                return Ok(delivery);
            }catch(ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateDelivery([FromBody] CreateDeliveryDTO createDeliveryDTO)
        {
            try
            {
                await _deliveryService.CreateDelivery(createDeliveryDTO);
                return Ok(createDeliveryDTO);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); 
            }
        }

        [HttpPut("/updatestatus")]
        public async Task<IActionResult> CompleteDelivery(int deliveryId, int orderId)
        {
            try
            {
                await _deliveryService.UpdateDeliveryStatus(deliveryId, orderId);
                return Ok("Delivery status updated successfully.");
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately (logging, returning error response, etc.)
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDelivery(int id)
        {
            _deliveryService.DeleteDelivery(id);
            return NoContent();
        }

        [HttpGet("order")]
        public async Task<ActionResult<List<ListOrderForDeliveryDTO>>> GetAllOrdersForDelivery()
        {
            try
            {
                var orders = await _deliveryService.GetAllOrdersAsync();
                return Ok(orders); // Returns 200 OK with the list of orders
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}"); // Returns 500 Internal Server Error
            }
        }
    }
}
