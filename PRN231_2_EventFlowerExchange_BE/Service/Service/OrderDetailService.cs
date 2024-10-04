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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static BusinessObject.Enum.EnumList;

namespace Service.Service
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public OrderDetailService(IOrderDetailRepository orderDetailRepository, IUserRepository userRepository, IMapper mapper)
        {
            _orderDetailRepository = orderDetailRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<List<ListOrderDetailDTO>> GetAllOrderDetail()
        {
            var orders = await _orderDetailRepository.GetAllOrdersDetail();
            var ordersDTO = _mapper.Map<List<ListOrderDetailDTO>>(orders);
            return ordersDTO;
        }

        public async Task<ListOrderDetailDTO> GetOrderDetailById(int orderId)
        {
            var orders = await _orderDetailRepository.GetOrderDetailById(orderId);
            ListOrderDetailDTO orderDTO = _mapper.Map<ListOrderDetailDTO>(orders);
            return orderDTO;
        }

        //public async Task Create(CreateOrderDTO orderDTO)
        //{
        //    // Kiểm tra các trường bắt buộc
        //    if (orderDTO == null || string.IsNullOrEmpty(orderDTO.DeliveryAddress) || orderDTO.CustomerId == 0)
        //    {
        //        throw new ArgumentException("All required fields must be filled");
        //    }

        //    // Kiểm tra chi tiết đơn hàng
        //    if (orderDTO.OrderDetails == null || !orderDTO.OrderDetails.Any())
        //    {
        //        throw new ArgumentException("Order must have at least one detail");
        //    }

        //    // Kiểm tra ngày tạo đơn hàng
        //    if (orderDTO.OrderDate == DateTime.MinValue)
        //    {
        //        throw new ArgumentException("Invalid order date");
        //    }

        //    // Kiểm tra ngày giao hàng
        //    if (orderDTO.DeliveryDate == DateTime.MinValue)
        //    {
        //        throw new ArgumentException("Invalid delivery date");
        //    }

        //    // Kiểm tra xem khách hàng có tồn tại không
        //    var customerExisting = await _userRepository.GetUserById(orderDTO.CustomerId);
        //    if (customerExisting == null)
        //    {
        //        throw new ArgumentException("Customer is not existed");
        //    }

        //    // Tính tổng giá từ OrderDetails
        //    var totalPrice = orderDTO.OrderDetails.Sum(detail => detail.QuantityOrdered * detail.Price);

        //    // Tìm các OrderDetail dựa trên danh sách OrderDetailId từ DTO
        //    var orderDetails = await _context.OrderDetails
        //        .Where(od => orderDTO.OrderDetails.Select(d => d.OrderDetailId).Contains(od.OrderDetailId))
        //        .ToListAsync();

        //    // Tạo đối tượng Order từ DTO
        //    Order newOrder = new Order
        //    {
        //        OrderStatus = OrderStatus.Pending,
        //        OrderDate = orderDTO.OrderDate,
        //        DeliveryAddress = orderDTO.DeliveryAddress,
        //        DeliveryDate = orderDTO.DeliveryDate,
        //        CustomerId = orderDTO.CustomerId,
        //        OrderDetails = orderDetails // Sử dụng các OrderDetail tìm được
        //    };

        //    // Gọi repository để tạo đơn hàng mới
        //    await _orderRepository.Create(newOrder);
        //}


        //public async Task Update(UpdateOrderDTO updateOrderDTO, int id)
        //{
        //    if (updateOrderDTO == null || string.IsNullOrEmpty(updateOrderDTO.OrderStatus)
        //     || string.IsNullOrEmpty(updateOrderDTO.DeliveryAddress)
        //     || updateOrderDTO.TotalPrice <= 0 || updateOrderDTO.CustomerId == 0 || updateOrderDTO.OrderDetails == 0)
        //    {
        //        throw new ArgumentException("All fields must be filled with valid values");
        //    }

        //    Regex statusRegex = new Regex(@"^(Pending|Shipped|Delivered|Cancelled)$");
        //    if (!statusRegex.IsMatch(updateOrderDTO.OrderStatus.ToString()))
        //    {
        //        throw new ArgumentException("Order status must be either Pending, Shipped, Delivered, or Cancelled!");
        //    }

        //    string[] dateFormats = { "dd/MM/yyyy", "dd/M/yyyy", "d/MM/yyyy", "d/M/yyyy" };
        //    DateTime orderDate, deliveryDate;
        //    if (!DateTime.TryParseExact(updateOrderDTO.OrderDate, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out orderDate))
        //    {
        //        throw new ArgumentException("Invalid order date format", nameof(updateOrderDTO.OrderDate));
        //    }
        //    if (!DateTime.TryParseExact(updateOrderDTO.DeliveryDate, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out deliveryDate))
        //    {
        //        throw new ArgumentException("Invalid delivery date format", nameof(updateOrderDTO.DeliveryDate));
        //    }

        //    Order existing = await _orderRepository.GetOrderById(id);
        //    if (existing == null)
        //    {
        //        throw new ArgumentException("Order does not exist");
        //    }

        //    var updateOrder = _mapper.Map<Order>(updateOrderDTO);
        //    await _orderRepository.Update(updateOrder, id);
        //}



        public async Task Delete(int orderId)
        {
            await _orderDetailRepository.Delete(orderId);
        }


    }
}
