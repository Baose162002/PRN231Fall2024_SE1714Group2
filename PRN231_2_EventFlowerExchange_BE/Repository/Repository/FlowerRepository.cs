using BusinessObject;
using Microsoft.EntityFrameworkCore;
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
            return await _context.Flowers.ToListAsync();
        }

        public async Task<Flower> GetFlowerById(int id)
        {
            return await _context.Flowers.FindAsync(id);
        }

        public async Task Create(Flower flower)
        {
            await _context.Flowers.AddAsync(flower);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Flower flower, int id)
        {
            var existingFlower = await _context.Flowers.FindAsync(id);
            if (existingFlower != null)
            {
                _context.Entry(existingFlower).CurrentValues.SetValues(flower);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Delete(int id)
        {
            var flower = await _context.Flowers.FindAsync(id); // Truy cập trực tiếp vào DbSet
            if (flower != null)
            {
                _context.Flowers.Remove(flower); // Truy cập trực tiếp vào DbSet
                await _context.SaveChangesAsync();
            }
        }
    }
}
