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
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly FlowerShopContext _context;
        public DeliveryRepository()
        {
            _context ??= new FlowerShopContext();

        }

        public async Task Create(Delivery delivery)
        {
            await _context.Deliveries.AddAsync(delivery);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery != null)
            {
                _context.Deliveries.Remove(delivery);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Delivery>> GetAllDeliveries() 
        {
            return await _context.Deliveries.ToListAsync();
        }
        public async Task<Delivery> GetDeliveryById(int id)
        {
            return await _context.Deliveries.FindAsync(id);
        }

        public async Task Update(Delivery delivery, int id)
        {
            var existing = await _context.Deliveries.FindAsync(id);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(delivery);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Order>> GetAllOrderForDelivery()
        {
            return await _context.Orders
                .Include(o => o.Customer) // Include User (Customer) information
                .Include(o => o.OrderDetails) // Include OrderDetails
                    .ThenInclude(od => od.Flower) // Include Flower information
                .ToListAsync();
        }

    }
}
