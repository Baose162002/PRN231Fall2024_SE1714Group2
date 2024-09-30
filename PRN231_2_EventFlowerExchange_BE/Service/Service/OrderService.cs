using AutoMapper;
using BusinessObject;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing;
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
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IUserRepository userRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
        }

        public async Task<List<ListOrderDTO>> GetAllOrder()
        {
            var orders = await _orderRepository.GetAllOrders();

            //// Mapping từ Order entity sang ListOrderDTO
            //var orderList = orders.Select(order => new ListOrderDTO
            //{
            //    OrderId = order.OrderId,
            //    OrderStatus = order.OrderStatus,
            //    TotalPrice = order.TotalPrice,
            //    OrderDate = order.OrderDate,
            //    DeliveryAddress = order.DeliveryAddress,
            //    DeliveryDate = order.DeliveryDate,
            //    //CustomerName = order.Customer.Name // Giả sử có thuộc tính Name trong User/Customer entity
            //}).ToList();
            var ordersDTO = _mapper.Map<List<ListOrderDTO>>(orders);
            return ordersDTO;
        }

        public async Task<ListOrderDTO> GetOrderById(int orderId)
        {
            var orders = await _orderRepository.GetOrderById(orderId);
            ListOrderDTO orderDTO = _mapper.Map<ListOrderDTO>(orders);
            return orderDTO;
        }

        public async Task Create(CreateOrderDTO orderDTO)
        {
            // Kiểm tra các trường bắt buộc
            if (orderDTO == null || string.IsNullOrEmpty(orderDTO.DeliveryAddress) || orderDTO.TotalPrice == 0 || orderDTO.CustomerId == 0)
            {
                throw new ArgumentException("All required fields must be filled");
            }

            // Kiểm tra giá tiền phải là số dương
            if (orderDTO.TotalPrice <= 0)
            {
                throw new ArgumentException("Total price must be a positive number");
            }

            // Kiểm tra chi tiết đơn hàng
            if (orderDTO.OrderDetails == null || !orderDTO.OrderDetails.Any())
            {
                throw new ArgumentException("Order must have at least one detail");
            }

            // Kiểm tra ngày tạo đơn hàng
            if (orderDTO.OrderDate == DateTime.MinValue)
            {
                throw new ArgumentException("Invalid order date");
            }

            // Kiểm tra ngày giao hàng
            if (orderDTO.DeliveryDate == DateTime.MinValue)
            {
                throw new ArgumentException("Invalid delivery date");
            }

            // Kiểm tra xem khách hàng có tồn tại không
            var customerExisting = await _userRepository.GetUserByIdAsync(orderDTO.CustomerId);
            if (customerExisting == null)
            {
                throw new ArgumentException("Customer is not existed");
            }

            // Tạo đối tượng Order từ DTO
            Order newOrder = new Order
            {
                OrderStatus = orderDTO.OrderStatus,
                TotalPrice = orderDTO.TotalPrice,
                OrderDate = orderDTO.OrderDate,
                DeliveryAddress = orderDTO.DeliveryAddress,
                DeliveryDate = orderDTO.DeliveryDate,
                CustomerId = orderDTO.CustomerId,
                OrderDetails = orderDTO.OrderDetails.Select(detail => new OrderDetail
                {
                    OrderDetailId = detail.FlowerId,
                    QuantityOrdered = detail.Quantity,
                    TotalPrice = detail.UnitPrice
                }).ToList()
            };

            // Gọi repository để tạo đơn hàng mới
            await _orderRepository.Create(_mapper.Map<Order>(newOrder));
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
