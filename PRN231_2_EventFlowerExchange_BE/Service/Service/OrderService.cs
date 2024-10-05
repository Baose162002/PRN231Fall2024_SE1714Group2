using AutoMapper;
using BusinessObject;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using BusinessObject.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing;
using Repository.IRepository;
using Repository.Repository;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Service.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBatchRepository _batchRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IUserRepository userRepository, IMapper mapper, IBatchRepository batchRepository, IOrderDetailRepository orderDetailRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _batchRepository = batchRepository;
            _orderDetailRepository = orderDetailRepository;
        }

        public OrderService()
        {
        }

        public async Task<List<ListOrderDTO>> GetAllOrder()
        {
            var orders = await _orderRepository.GetAllOrders();
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
            if (orderDTO == null || string.IsNullOrEmpty(orderDTO.DeliveryAddress) || orderDTO.CustomerId == 0 || orderDTO.QuantityOrdered <= 0)
            {
                throw new ArgumentException("All required fields must be filled");
            }

            List<OrderDetail> orderDetails = new List<OrderDetail>();
            decimal totalPrice = 0;

            // Lấy danh sách các Batch có chứa FlowerId tương ứng
            var flowerBatches = await _batchRepository.GetAvailableBatchesByFlowerId(orderDTO.FlowerId);

            if (flowerBatches == null || !flowerBatches.Any())
            {
                throw new ArgumentException("No batches available for this flower.");
            }

            int remainingQuantity = orderDTO.QuantityOrdered; // Số lượng cần đặt

            foreach (var batch in flowerBatches)
            {
                if (remainingQuantity <= 0) break; // Nếu đã đủ số lượng thì thoát

                int batchQuantityToUse = Math.Min(batch.BatchQuantity, remainingQuantity); // Lấy số lượng ít hơn giữa lô và còn lại

                // Tạo đối tượng OrderDetail
                var orderDetail = new OrderDetail
                {
                    BatchId = batch.BatchId,
                    QuantityOrdered = batchQuantityToUse,
                    Price = batch.PricePerUnit,
                    TotalPrice = batchQuantityToUse * batch.PricePerUnit // Gán giá trị TotalPrice trước khi lưu
                };

                // Tính tổng giá của OrderDetail và cập nhật tổng giá của Order
                totalPrice += orderDetail.TotalPrice;

                // Thêm OrderDetail vào danh sách
                orderDetails.Add(orderDetail);

                // Cập nhật số lượng còn lại trong Batch
                batch.BatchQuantity -= batchQuantityToUse;
                await _batchRepository.UpdateBatch(batch);

                // Giảm số lượng còn lại cần đặt
                remainingQuantity -= batchQuantityToUse;
            }

            if (remainingQuantity > 0)
            {
                throw new ArgumentException("Not enough stock to fulfill the order.");
            }

            // Tạo đối tượng Order
            var newOrder = new Order
            {
                OrderStatus = EnumList.OrderStatus.Pending,
                OrderDate = orderDTO.OrderDate,
                DeliveryAddress = orderDTO.DeliveryAddress,
                DeliveryDate = orderDTO.DeliveryDate,
                CustomerId = orderDTO.CustomerId,
                TotalPrice = totalPrice,
                OrderDetails = orderDetails
            };

            // Tạo đơn hàng mới
            await _orderRepository.Create(newOrder);
        }

        public async Task Update(UpdateOrderDTO updateOrderDTO, int id)
        {
            if (updateOrderDTO == null || string.IsNullOrEmpty(updateOrderDTO.OrderStatus)
             || string.IsNullOrEmpty(updateOrderDTO.DeliveryAddress)
             || updateOrderDTO.TotalPrice <= 0 || updateOrderDTO.CustomerId == 0 || updateOrderDTO.OrderDetails == 0)
            {
                throw new ArgumentException("All fields must be filled with valid values");
            }

            Regex statusRegex = new Regex(@"^(Pending|Confirmed|Dispatched|Delivered)$");
            if (!statusRegex.IsMatch(updateOrderDTO.OrderStatus.ToString()))
            {
                throw new ArgumentException("Order status must be either Pending, Confirmed, Dispatched, or Delivered!");
            }

            string[] dateFormats = { "dd/MM/yyyy", "dd/M/yyyy", "d/MM/yyyy", "d/M/yyyy" };
            DateTime orderDate, deliveryDate;
            if (!DateTime.TryParseExact(updateOrderDTO.OrderDate, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out orderDate))
            {
                throw new ArgumentException("Invalid order date format", nameof(updateOrderDTO.OrderDate));
            }
            if (!DateTime.TryParseExact(updateOrderDTO.DeliveryDate, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out deliveryDate))
            {
                throw new ArgumentException("Invalid delivery date format", nameof(updateOrderDTO.DeliveryDate));
            }

            Order existing = await _orderRepository.GetOrderById(id);
            if (existing == null)
            {
                throw new ArgumentException("Order does not exist");
            }

            var updateOrder = _mapper.Map<Order>(updateOrderDTO);
            await _orderRepository.Update(updateOrder, id);
        }



        public async Task Delete(int orderId)
        {
            await _orderRepository.Delete(orderId);
        }

        public async Task<Order> UpdateOrderStatus(int updatedOrder)
        {
            return _orderRepository.UpdateOrderStatusAsync(updatedOrder);
        }

        public Task<List<Order>> SearchOrders(OrderSearchDTO searchCriteria)
        {
            return _orderRepository.SearchOrders(searchCriteria);
        }
    }
}
