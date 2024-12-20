﻿using BusinessObject;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.IService;
using System.Security.Claims;
using static BusinessObject.Enum.EnumList;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Formatter;

namespace WebApi_EventFlowerExchange.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IFlowerService _flowerService;
        private readonly IBatchService _batchService;
        private readonly string[] _orderStatusValues = { "Pending", "Confirmed", "Dispatched", "Delivered" };


        public OrderController(IOrderService orderService, IFlowerService flowerService, IBatchService batchService)
        {
            _orderService = orderService;
            _flowerService = flowerService;
            _batchService = batchService;
        }



        // GET: odata/Order
        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var orders = await _orderService.GetAllOrder();
            if (orders == null || !orders.Any())
            {
                return NotFound("Order list is empty");
            }
            return Ok(orders);
        }

        // Lấy Order của người dùng đó
        [HttpGet("user")]
        [Authorize(Roles = "Seller, Buyer")]
        public async Task<IActionResult> GetAllOrdersByUserId(string userRole,string userId)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role == null)
            {
                role = userRole;
            }
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (id == null)
            {
                id = userId;
            }

            var orders = await _orderService.GetAllOrdersByUserId(userRole: role, int.Parse(id));

            if (orders == null || !orders.Any())
            {
                return NotFound("You don't have any order!");
            }

            return Ok(orders);
        }


        // GET: api/Order/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderById(id);
            if (order == null)
            {
                return NotFound("Order does not exist");
            }
            return Ok(order);
        }

        // POST: api/Order
        [HttpPost("quick-order")]
        public async Task<IActionResult> CreateOrder(CreateOrderDTO createOrderDTO)
        {
            try
            {
                await _orderService.Create(createOrderDTO);
                return Ok("Order created successfully");
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost("order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderFlowerDTO createOrder)
        {
            try
            {
                // Gọi service để tạo đơn hàng và nhận về OrderId
                var orderId = await _orderService.CreateOrder(createOrder);

                // Trả về kết quả thành công kèm theo OrderId
                var result = new CreateOrderResponse
                {
                    Status = "success",
                    Message = "Create Order successfully",
                    OrderId = orderId
                };
                return Ok(result);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }



        /*    [HttpPost]
            public async Task<IActionResult> CreateOrderByBatch([FromBody] CreateOrderDTO createOrderDTO)
            {
                try
                {
                    await _orderService.CreateOrderByBatch(createOrderDTO);
                    return Ok("Order created successfully");
                }
                catch (ArgumentException e)
                {
                    return BadRequest(e.Message);
                }
            }*/

        // PUT: api/Order/id
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder([FromBody] UpdateOrderDTO updateOrderDTO, int id)
        {
            try
            {
                await _orderService.Update(updateOrderDTO, id);
                return Ok("Order updated successfully");
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: api/Order/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                await _orderService.Delete(id);
                return NoContent(); // Trả về mã 204 No Content
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message); // Trả về mã 404 Not Found
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // Trả về mã 400 Bad Request
            }
        }


        [HttpPut("update-status")]
        public IActionResult UpdateOrderStatus([FromBody] UpdateOrderStatusDTO request)
        {
            var result = _orderService.UpdateOrderStatus(request.OrderId);
            if (result == null)
            {
                return NotFound("Order not found or already at the last status.");
            }
            return Ok(result);
        }

        [HttpPut("update-status-vnpay/{id}")]
        public async Task<IActionResult> UpdateStatus([FromBody] OrderUpdateStatusDTO order, int id)
        {
            try
            {
                await _orderService.UpdateStatus(order, id);
                return Ok("Order status updated successfully");
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Log error here
                return StatusCode(500, "An error occurred while updating the order status");
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchOrders([FromQuery] OrderSearchDTO searchCriteria)
        {
            var orders = await _orderService.SearchOrders(searchCriteria);
            return Ok(orders);
        }

    }
}
