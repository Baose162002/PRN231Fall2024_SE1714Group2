using BusinessObject;
using BusinessObject.Enum;
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
        public async Task<List<Delivery>> GetDeliveriesByPersonnelId(int id)
        {
            return await _context.Deliveries
                                 .Include(d => d.Order)               // Include the Order related to each Delivery
                                 .ThenInclude(o => o.Customer)           // Include the User related to each Order
                                 .Where(d => d.DeliveryPersonnelId == id)
                                 .ToListAsync();
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
                .Where(o => o.OrderStatus == OrderStatus.Paid) // Filter for orders with status 'Paid'
                .ToListAsync();
        }

        public async Task<bool> ExistsOrder(int orderId)
        {
            return await _context.Orders.AnyAsync(o => o.OrderId == orderId);
        }
        public async Task<bool> ExistsUser(int deliveryPersonnelId)
        {
            return await _context.Users.AnyAsync(u => u.UserId == deliveryPersonnelId);
        }

        public async Task UpdateOrderStatus(int orderId)
        {
            // Fetch the order from the context using its ID
            var order = await _context.Orders.FindAsync(orderId); // Replace Orders with the actual DbSet for your orders

            // Check if the order exists
            if (order == null)
            {
                throw new ArgumentException($"Order with ID {orderId} does not exist.");
            }

            // Update the order status
            order.OrderStatus = OrderStatus.InTransit;

            // Save changes to the database
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsDeliveryForOrder(int orderId)
        {
            return await _context.Deliveries
                .AnyAsync(d => d.OrderId == orderId);
        }
        public async Task UpdateDeliveryStatus(int deliveryId, int orderId)
        {
            // Retrieve the delivery and order based on the provided IDs
            var delivery = await _context.Deliveries.FindAsync(deliveryId);
            var order = await _context.Orders.FindAsync(orderId);

            // Check if both entities exist
            if (delivery == null)
            {
                throw new ArgumentException("Delivery not found.");
            }

            if (order == null)
            {
                throw new ArgumentException("Order not found.");
            }

            // Update the statuses
            order.OrderStatus = EnumList.OrderStatus.ShippingCompleted;
            delivery.DeliveryStatus = EnumList.DeliveryStatus.Complete;

            // Update the entities in the context
            _context.Orders.Update(order);
            _context.Deliveries.Update(delivery);

            // Save changes to the database
            await _context.SaveChangesAsync();
        }
    }
}
