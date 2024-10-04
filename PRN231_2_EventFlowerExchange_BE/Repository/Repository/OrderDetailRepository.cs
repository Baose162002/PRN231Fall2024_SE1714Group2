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
    public class OrderDetailRepository : IOrderDetailRepository
    {
        public async Task<List<OrderDetail>> GetAllOrdersDetail()
        {
            var _context = new FlowerShopContext();
            var orders = await _context.OrderDetails
                .ToListAsync();
            return orders;
        }

        public async Task<OrderDetail> GetOrderDetailById(int id)
        {
            var _context = new FlowerShopContext();
            var existing = await _context.OrderDetails
                .FirstOrDefaultAsync(o => o.OrderId == id);
            return existing;
        }

        public async Task Create(OrderDetail order)
        {
            var _context = new FlowerShopContext();
            _context.OrderDetails.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task Update(OrderDetail order, int id)
        {
            var _context = new FlowerShopContext();
            var existing = await GetOrderDetailById(id);
            _context.OrderDetails.Update(existing);
            await _context.SaveChangesAsync();

        }


        public async Task Delete(int id)
        {
            var _context = new FlowerShopContext();
            var existing = await GetOrderDetailById(id);
            if (existing == null)
            {
                throw new ArgumentException("Order is not existed");
            }
            _context.OrderDetails.Remove(existing);
            await _context.SaveChangesAsync();
        }




    }
}
