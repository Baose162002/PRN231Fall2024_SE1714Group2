using AutoMapper;
using BusinessObject;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using Repository.Repository;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<ListOrderDTO>> GetAllOrder()
        {
            var orders = await _orderRepository.GetAllOrders();

            // Mapping từ Order entity sang ListOrderDTO
            var orderList = orders.Select(order => new ListOrderDTO
            {
                OrderId = order.OrderId,
                OrderStatus = order.OrderStatus,
                TotalPrice = order.TotalPrice,
                OrderDate = order.OrderDate,
                DeliveryAddress = order.DeliveryAddress,
                DeliveryDate = order.DeliveryDate,
                //CustomerName = order.Customer.Name // Giả sử có thuộc tính Name trong User/Customer entity
            }).ToList();

            return orderList;
        }

        public async Task<ListOrderDTO> GetOrderById(int orderId)
        {
            var orders = await _orderRepository.GetOrderById(orderId);
            ListOrderDTO orderDTO = _mapper.Map<ListOrderDTO>(orders);
            return orderDTO;
        }

        public async Task Create(CreateOrderDTO order)
        {
            var createOrder = _mapper.Map<Order>(order);
            await _orderRepository.Create(createOrder);
        }

        public async Task Update(UpdateOrderDTO order, int id)
        {
            var existing = await _orderRepository.GetOrderById(id);
            if (existing != null)
            {
                // Cập nhật các thuộc tính của đơn hàng từ đối tượng `order` mới
                existing.OrderStatus = order.OrderStatus;
                existing.TotalPrice = order.TotalPrice;
                existing.OrderDate = order.OrderDate;
                existing.DeliveryAddress = order.DeliveryAddress;
                existing.DeliveryDate = order.DeliveryDate;
                existing.CustomerId = order.CustomerId;

                // Cập nhật chi tiết đơn hàng nếu cần
                // existing.OrderDetails = order.OrderDetails; // Tùy chỉnh theo yêu cầu

                var updateOrder = _mapper.Map<Order>(order);
                await _orderRepository.Update(updateOrder, id);
            }
            else
            {
                throw new ArgumentException("Order is not existed");
            }
        }


        public async Task Delete(int orderId)
        {
            await _orderRepository.Delete(orderId);
        }


    }
}
