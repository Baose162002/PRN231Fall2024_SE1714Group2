﻿using BusinessObject;
using BusinessObject.DTO.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.IService;
using static BusinessObject.Enum.EnumList;

namespace WebApi_EventFlowerExchange.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly string[] _orderStatusValues = { "Pending", "Confirmed", "Dispatched", "Delivered" };


        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: api/Order
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrder();
            if (orders == null || !orders.Any())
            {
                return NotFound("Order list is empty");
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
        [HttpPost]
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

        [HttpGet("search")]
        public async Task<IActionResult> SearchOrders([FromQuery] OrderSearchDTO searchCriteria)
        {
            var orders = await _orderService.SearchOrders(searchCriteria);
            return Ok(orders);
        }

    }
}
