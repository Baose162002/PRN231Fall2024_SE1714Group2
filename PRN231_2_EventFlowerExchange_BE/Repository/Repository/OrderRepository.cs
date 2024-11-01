using BusinessObject;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.Enum.EnumList;

namespace Repository.Repository
{
    public class OrderRepository : IOrderRepository
    {
        public async Task<List<Order>> GetAllOrders()
        {
            var _context = new FlowerShopContext();
            var orders = await _context.Orders
                .Include(o => o.Customer) // Include related Customer entity
                .Include(o => o.OrderDetails) // Include OrderDetails
                .ThenInclude(b => b.Flower) // Include Flower within Batch
                .ToListAsync();
            return orders;
        }

        public async Task<List<Order>> GetAllOrdersByUserId(int id)
        {
            var _context = new FlowerShopContext();
            var orders = await _context.Orders
                .Include(o => o.Customer) // Include related Customer entity
                .Include(o => o.OrderDetails) // Include OrderDetails
                .ThenInclude(b => b.Flower) // Include Flower within Batch
                .Where(r => r.CustomerId == id)
                .ToListAsync();
            return orders;
        }

        // Get orders related to batches of the seller (UserId == 2)
        public async Task<List<Order>> GetOrdersBySellerBatch(int sellerId)
        {
            var _context = new FlowerShopContext();
            var orders = await _context.Orders
                .Include(o => o.Customer) // Include related Customer entity
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Flower)
                .ThenInclude(f => f.Batch)
                .Where(o => o.OrderDetails.Any(od => od.Flower.Batch.CompanyId == sellerId))
                .ToListAsync();
            return orders;
        }

        public async Task<Order> GetOrderById(int id)
        {
            var _context = new FlowerShopContext();
            var existing = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .ThenInclude(b => b.Flower) // Include Flower within Batch
                .FirstOrDefaultAsync(o => o.OrderId == id);
            return existing;
        }

        public async Task Create(Order order)
        {
            var _context = new FlowerShopContext();
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Order order, int id)
        {
            var _context = new FlowerShopContext();
            var existing = await GetOrderById(id);
            if (existing != null)
            {
                // Cập nhật các thuộc tính của đơn hàng từ đối tượng `order` mới
                existing.OrderStatus = order.OrderStatus;
                existing.TotalPrice = order.TotalPrice;
                existing.OrderDate = order.OrderDate;
                existing.DeliveryAddress = order.DeliveryAddress;
                existing.DeliveryDate = order.DeliveryDate;
                existing.CustomerId = order.CustomerId;

                _context.Orders.Update(existing);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("Order is not existed");
            }
        }

        public async Task Delete(int id)
        {
            var _context = new FlowerShopContext();
            var existing = await GetOrderById(id);
            if (existing == null)
            {
                throw new ArgumentException("Order is not existed");
            }
            // Kiểm tra trạng thái đơn hàng
            if (existing.OrderStatus != OrderStatus.Pending)
            {
                throw new InvalidOperationException("Cannot delete order that is not pending.");
            }

            // Kiểm tra xem đơn hàng có chứa OrderDetail hay không
            if (existing.OrderDetails != null && existing.OrderDetails.Any())
            {
                throw new InvalidOperationException("Cannot delete order that contains order details.");
            }
            _context.Orders.Remove(existing);
            await _context.SaveChangesAsync();
        }

        public Order UpdateOrderStatusAsync(int orderId)
        {
            var _context = new FlowerShopContext();
            var order = _context.Orders.Find(orderId);
            if (order == null || order.OrderStatus == OrderStatus.ShippingCompleted)
            {
                return null; // Trả về null nếu không tìm thấy hoặc đã đến trạng thái cuối cùng
            }

            // Tăng giá trị OrderStatus
            order.OrderStatus += 1; // Chuyển sang trạng thái tiếp theo

            _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

            return order; // Trả về đơn hàng đã cập nhật
        }

        public async Task<List<Order>> SearchOrders(OrderSearchDTO searchCriteria)
        {
            var _context = new FlowerShopContext();
            var query = _context.Orders.AsQueryable();

            if (searchCriteria.Status.HasValue)
            {
                query = query.Where(o => o.OrderStatus == searchCriteria.Status.Value);
            }

            if (searchCriteria.OrderDateFrom.HasValue)
            {
                query = query.Where(o => o.OrderDate >= searchCriteria.OrderDateFrom.Value);
            }

            if (searchCriteria.OrderDateTo.HasValue)
            {
                query = query.Where(o => o.OrderDate <= searchCriteria.OrderDateTo.Value);
            }

            if (searchCriteria.CustomerId.HasValue)
            {
                query = query.Where(o => o.CustomerId == searchCriteria.CustomerId.Value);
            }

            return await query.ToListAsync();
        }



        public async Task UpdateStatus(Order order, int id)
        {
            var _context = new FlowerShopContext();
            var existing = await GetOrderById(id);
            if (existing != null)
            {
                existing.OrderStatus = order.OrderStatus;
                _context.Orders.Update(existing);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("Order is not existed");
            }
        }
    }
}
