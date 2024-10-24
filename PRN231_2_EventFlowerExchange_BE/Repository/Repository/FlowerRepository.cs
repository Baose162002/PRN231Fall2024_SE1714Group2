using BusinessObject;
using Microsoft.EntityFrameworkCore;
using BusinessObject.Enum;
using Repository.IRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class FlowerRepository : IFlowerRepository
    {
        private readonly FlowerShopContext _context;
        public FlowerRepository()
        {
            _context ??= new FlowerShopContext();
        }

        public async Task<List<Flower>> GetAllFlowers()
        {
            return await _context.Flowers.Include(e => e.Batch).ThenInclude(b => b.Company).ToListAsync();
        }

        public async Task<Flower> GetFlowerById(int id)
        {
            var existing = await _context.Flowers.Include(e => e.Batch).ThenInclude(b => b.Company).FirstOrDefaultAsync(e => e.FlowerId == id);
            return existing;
        }

        public async Task Create(Flower flower)
        {
            await _context.Flowers.AddAsync(flower);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Flower flower, int id)
        {
            var _context = new FlowerShopContext();

            // Fetch the existing flower
            var existing = await _context.Flowers.FindAsync(id);
            if (existing == null)
            {
                throw new ArgumentException("Flower not found.");
            }

            // Fetch the associated batch
            var batch = await _context.Batches.FindAsync(existing.BatchId);
            if (batch == null)
            {
                throw new ArgumentException("Batch not found.");
            }

            // Store the old remaining quantity before updating
            int oldRemainingQuantity = existing.RemainingQuantity;

            // Update the flower properties
            existing.Name = flower.Name;
            existing.Type = flower.Type;
            existing.Image = flower.Image;
            existing.Description = flower.Description;
            existing.PricePerUnit = flower.PricePerUnit;
            existing.Origin = flower.Origin;
            existing.Color = flower.Color;
            existing.RemainingQuantity = flower.RemainingQuantity; // This is the new quantity
            existing.Condition = EnumList.FlowerCondition.Fresh; // Update condition
            existing.FlowerStatus = flower.RemainingQuantity > 0 ? EnumList.FlowerStatus.Available : EnumList.FlowerStatus.SoldOut;
            existing.Status = EnumList.Status.Active;
            _context.Flowers.Update(existing);
            await _context.SaveChangesAsync();
        }

      


        public async Task Delete(int id)
        {
            var existing = await GetFlowerById(id);
            if(existing == null)
            {
                throw new ArgumentException("Flower is not existed");
            }
            existing.Status = EnumList.Status.Inactive;
            _context.Flowers.Update(existing);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateFlower(Flower flower)
        {
            var _context = new FlowerShopContext();
            _context.Flowers.Update(flower);
            await _context.SaveChangesAsync();
        }
    }
}
