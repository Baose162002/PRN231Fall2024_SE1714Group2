using BusinessObject;
using BusinessObject.Enum;
using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class BatchRepository : IBatchRepository
    {
        public async Task<List<Batch>> GetAllBatch()
        {
            var _context = new FlowerShopContext();
            var batches = await _context.Batches
               .Include(e => e.Company)   
               .Include(e => e.Flower) 
               .ToListAsync();
            return batches;
        }

        public async Task<Batch> GetBatchById(int id)
        {
            var _context = new FlowerShopContext();
            var existing = await _context.Batches.Include(e => e.Company).Include(e => e.Flower).FirstOrDefaultAsync(b => b.BatchId == id);
            return existing;
        }
        public async Task<Flower> GetFlowerById(int id)
        {
            var _context = new FlowerShopContext();
            var existing = await _context.Flowers.FirstOrDefaultAsync(b => b.FlowerId == id);
            return existing;
        }

        public async Task Create(Batch batch)
        {
            var _context = new FlowerShopContext();
            _context.Batches.Add(batch);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Batch batch, int id)
        {
            var _context = new FlowerShopContext();
            var existing = await GetBatchById(id);
            if(existing != null)
            {
                existing.FlowerType = batch.FlowerType;
                existing.BatchQuantity = batch.BatchQuantity;
                existing.ImgFlower = batch.ImgFlower;
                existing.Description = batch.Description;
                existing.PricePerUnit = batch.PricePerUnit;
                existing.Condition = batch.Condition;
                existing.EntryDate = batch.EntryDate;
                existing.BatchStatus = batch.BatchStatus;
                existing.FlowerId = batch.FlowerId;
            }
            _context.Batches.Update(existing);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var _context = new FlowerShopContext();
            var existing = await GetBatchById(id);
            if(existing == null)
            {
                throw new ArgumentException("Batch is not existed");
            }
            _context.Batches.Remove(existing);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Batch>> GetAvailableBatchesByFlowerId(int flowerId)
        {
            var _context = new FlowerShopContext();
            return await _context.Batches
                .Where(b => b.FlowerId == flowerId && b.BatchQuantity > 0 && b.BatchStatus == EnumList.BatchStatus.Available)
                .OrderBy(b => b.EntryDate) // Sắp xếp theo ngày nhập kho
                .ToListAsync();
        }

        public async Task UpdateBatch(Batch batch)
        {
            var _context = new FlowerShopContext();
            _context.Batches.Update(batch);
            await _context.SaveChangesAsync();
        }
    }
}
