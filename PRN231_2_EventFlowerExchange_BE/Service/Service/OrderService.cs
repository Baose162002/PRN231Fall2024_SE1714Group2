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
        private readonly IFlowerRepository _flowerRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IUserRepository userRepository, IMapper mapper, IBatchRepository batchRepository, IOrderDetailRepository orderDetailRepository, IFlowerRepository flowerRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _batchRepository = batchRepository;
            _orderDetailRepository = orderDetailRepository;
            _flowerRepository = flowerRepository;
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
                throw new ArgumentException("All required fields must be filled.");
            }

            List<OrderDetail> orderDetails = new List<OrderDetail>();
            double totalPrice = 0;

            // Lấy danh sách các Batch có chứa FlowerId tương ứng
            var flowerBatches = await _batchRepository.GetFlowersBySimilarTypeAndColorAndEarliestBatch(orderDTO.FlowerId);

            if (flowerBatches == null || !flowerBatches.Any())
            {
                throw new ArgumentException("No batches available for this flower.");
            }

            // Lấy thông tin hoa để kiểm tra số lượng
            var flower = await _flowerRepository.GetFlowerById(orderDTO.FlowerId); // Lấy thông tin hoa theo ID
            if (flower == null)
            {
                throw new ArgumentException("Flower not found.");
            }

            // Kiểm tra số lượng đặt hàng không vượt quá số lượng còn lại của hoa
            if (orderDTO.QuantityOrdered > flower.RemainingQuantity)
            {
                throw new ArgumentException("Order quantity exceeds available quantity of the flower.");
            }

            int remainingQuantity = orderDTO.QuantityOrdered; // Số lượng cần đặt

            // Lặp qua các lô hoa để phân bổ số lượng đặt hàng
            foreach (var flowers in flowerBatches)
            {
                if (remainingQuantity <= 0) break; // Nếu đã đủ số lượng thì thoát

                int batchQuantityToUse = Math.Min(flowers.RemainingQuantity, remainingQuantity); // Lấy số lượng ít hơn giữa lô và còn lại

                // Tạo đối tượng OrderDetail
                var orderDetail = new OrderDetail
                {
                    FlowerId = flowers.FlowerId, // Lấy BatchId
                    QuantityOrdered = batchQuantityToUse,
                    Price = flowers.PricePerUnit, // Lấy giá từ hoa
                    TotalPrice = batchQuantityToUse * flowers.PricePerUnit // Tính tổng giá của OrderDetail
                };

                // Tính tổng giá của OrderDetail và cập nhật tổng giá của Order
                totalPrice += orderDetail.TotalPrice;

                // Thêm OrderDetail vào danh sách
                orderDetails.Add(orderDetail);

                // Giảm số lượng còn lại cần đặt
                remainingQuantity -= batchQuantityToUse;
            }

            // Nếu sau khi phân bổ các lô mà vẫn còn số lượng chưa đủ, ném lỗi
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

            // Cập nhật số lượng còn lại của hoa
            flower.RemainingQuantity -= orderDTO.QuantityOrdered; // Chỉ cập nhật RemainingQuantity bên hoa
            await _flowerRepository.UpdateFlower(flower); // Cập nhật hoa
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

        /*  //Tạo Order bằng cách chọn Flower tùy thuộc vào Batch
          public async Task CreateOrderByBatch(CreateOrderDTO orderDTO)
          {
              // Tìm tất cả các Batch có chứa loại hoa này (FlowerId)
              var availableBatches = await _batchRepository.GetAvailableBatchesByFlowerId(orderDTO.FlowerId);

              // Lấy danh sách BatchId hợp lệ từ FlowerId đã chọn
              var validBatchIds = availableBatches.Select(b => b.BatchId).ToList();

              // Kiểm tra xem các BatchId được chọn có hợp lệ không
              foreach (var selectedBatch in orderDTO.SelectedBatches)
              {
                  if (!validBatchIds.Contains(selectedBatch.BatchId))
                  {
                      throw new ArgumentException($"BatchId {selectedBatch.BatchId} is not valid for FlowerId {orderDTO.FlowerId}");
                  }

                  // Lấy Batch từ cơ sở dữ liệu
                  var batch = await _batchRepository.GetBatchById(selectedBatch.BatchId);

                  // Kiểm tra số lượng hoa trong batch có đủ để đặt không
                  if (batch.BatchQuantity < selectedBatch.QuantityOrdered)
                  {
                      throw new ArgumentException($"Insufficient quantity in Batch {selectedBatch.BatchId}. Available: {batch.BatchQuantity}");
                  }

                  // Cập nhật lại số lượng bông trong Batch
                  batch.BatchQuantity -= selectedBatch.QuantityOrdered;

                  // Lưu thay đổi vào cơ sở dữ liệu sau khi cập nhật BatchQuantity
                  await _batchRepository.UpdateBatch(batch);
              }

              // Tính tổng giá trị đơn hàng (TotalPrice)
              decimal totalPrice = 0;
              var orderDetails = new List<OrderDetail>();
              foreach (var selectedBatch in orderDTO.SelectedBatches)
              {
                  var batch = await _batchRepository.GetBatchById(selectedBatch.BatchId);
                  var orderDetail = new OrderDetail
                  {
                      BatchId = selectedBatch.BatchId,
                      QuantityOrdered = selectedBatch.QuantityOrdered,
                      Price = batch.PricePerUnit
                  };
                  orderDetails.Add(orderDetail);

                  totalPrice += selectedBatch.QuantityOrdered * batch.PricePerUnit;
              }

              // Tạo đơn hàng (Order)
              var order = new Order
              {
                  OrderStatus = EnumList.OrderStatus.Pending,
                  TotalPrice = totalPrice,
                  OrderDate = DateTime.Now,
                  DeliveryAddress = "Sample Address", // Nhập thông tin từ DTO nếu cần
                  DeliveryDate = DateTime.Now.AddDays(3), // Giả sử ngày giao hàng sau 3 ngày
                  CustomerId = 1, // Thay thế bằng ID người dùng thực tế
                  OrderDetails = orderDetails
              };

              // Lưu vào DB
              await _orderRepository.Create(order);
          }*/

        public async Task CreateOrder(CreateOrderFlowerDTO orderDTO)
        {
            // Kiểm tra các trường bắt buộc
            if (orderDTO == null || string.IsNullOrEmpty(orderDTO.DeliveryAddress) || orderDTO.CustomerId == 0 || orderDTO.QuantityOrdered <= 0)
            {
                throw new ArgumentException("All required fields must be filled");
            }

            List<OrderDetail> orderDetails = new List<OrderDetail>();
            double totalPrice = 0;

            // Lấy danh sách các lô hoa theo loại và màu sắc tương tự
            var flowerBatches = await _batchRepository.GetFlowersBySimilarTypeAndColorAndEarliestBatchAndCompany(orderDTO.FlowerId);

            if (flowerBatches == null || !flowerBatches.Any())
            {
                throw new ArgumentException("No batches available for this flower.");
            }

            // Lấy thông tin hoa để kiểm tra số lượng và các thuộc tính khác
            var flower = await _flowerRepository.GetFlowerById(orderDTO.FlowerId);

            if (flower == null)
            {
                throw new ArgumentException("Flower not found.");
            }

            // Kiểm tra số lượng đặt hàng không vượt quá số lượng còn lại của hoa
            if (orderDTO.QuantityOrdered > flower.RemainingQuantity)
            {
                throw new ArgumentException("Order quantity exceeds available quantity of the flower.");
            }

            // Kiểm tra trạng thái hoa để đảm bảo hoa có thể được đặt
            if (flower.FlowerStatus != EnumList.FlowerStatus.Available ||
                (flower.Condition != EnumList.FlowerCondition.Fresh))
            {
                throw new ArgumentException("Flower is not available for order.");
            }

            int remainingQuantity = orderDTO.QuantityOrdered; // Số lượng cần đặt

            // Duyệt qua các lô hoa để tạo OrderDetail cho từng lô hoa
            foreach (var batch in flowerBatches)
            {
                if (remainingQuantity <= 0) break; // Nếu đủ số lượng thì dừng

                // Lấy số lượng hoa từ lô, số lượng lấy không vượt quá số lượng còn lại trong lô
                int batchQuantityToUse = Math.Min(batch.RemainingQuantity, remainingQuantity);

                // Tạo đối tượng OrderDetail cho lô hoa này
                var orderDetail = new OrderDetail
                {
                    FlowerId = batch.FlowerId,
                    QuantityOrdered = batchQuantityToUse,
                    Price = flower.PricePerUnit,
                    TotalPrice = batchQuantityToUse * flower.PricePerUnit
                };

                // Cập nhật tổng giá trị đơn hàng
                totalPrice += orderDetail.TotalPrice;
                flower.RemainingQuantity -= batchQuantityToUse; // Cập nhật số lượng hoa còn lại trong flower

                // Thêm chi tiết đơn hàng vào danh sách
                orderDetails.Add(orderDetail);

                // Giảm số lượng còn lại cần đặt
                remainingQuantity -= batchQuantityToUse;
            }

            // Nếu vẫn còn số lượng chưa đặt đủ, báo lỗi
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

            // Cập nhật số lượng còn lại của hoa sau khi đơn hàng được tạo
            await _flowerRepository.UpdateFlower(flower);

            // Kiểm tra và cập nhật trạng thái hoa
            if (flower.RemainingQuantity <= 0)
            {
                flower.FlowerStatus = EnumList.FlowerStatus.SoldOut; // Cập nhật trạng thái hoa thành SoldOut
                await _flowerRepository.UpdateFlower(flower); // Cập nhật lại thông tin hoa trong DB
            }
        }

    }
}