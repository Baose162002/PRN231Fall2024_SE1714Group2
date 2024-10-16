using AutoMapper;
using BusinessObject.DTO.Request;
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
        public IActionResult GetAllDeliveries()
        {
            var deliveries = _deliveryService.GetAllDeliveries();
            return Ok(deliveries);
        }

        [HttpGet("{id}")]
        public IActionResult GetDeliveryById(int id)
        {
            var delivery = _deliveryService.GetDeliveryById(id);
            if (delivery == null)
            {
                return NotFound();
            }
            return Ok(delivery);
        }

        [HttpPost]
        public IActionResult CreateDelivery([FromBody] CreateDeliveryDTO createDeliveryDTO)
        {
            _deliveryService.CreateDelivery(createDeliveryDTO);
            return CreatedAtAction(nameof(GetDeliveryById), new { id = createDeliveryDTO.OrderId }, createDeliveryDTO);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateDelivery([FromBody] UpdateDeliveryDTO updateDeliveryDTO, int id)
        {
            _deliveryService.UpdateDelivery(updateDeliveryDTO, id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDelivery(int id)
        {
            _deliveryService.DeleteDelivery(id);
            return NoContent();
        }
    }
}
