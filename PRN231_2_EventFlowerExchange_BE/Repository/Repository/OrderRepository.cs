using BusinessObject;
using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class OrderRepository : IOrderRepository
    {
        public async Task<List<Order>> GetAllOrders()
        {
            var _context = new FlowerShopContext();
            var orders = await _context.Orders
                .Include(o => o.Customer)
                //.Include(o => o.OrderDetails)
                //.ThenInclude(od => od.Flower) // Giả sử OrderDetail có liên kết với Flower
                .ToListAsync();
            return orders;
        }

        public async Task<Order> GetOrderById(int id)
        {
            var _context = new FlowerShopContext();
            var existing = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                //.ThenInclude(od => od.Flower) // Giả sử OrderDetail có liên kết với Flower
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

                // Cập nhật chi tiết đơn hàng nếu cần
                // existing.OrderDetails = order.OrderDetails; // Tùy chỉnh theo yêu cầu

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
            _context.Orders.Remove(existing);
            await _context.SaveChangesAsync();
        }




    }
}
