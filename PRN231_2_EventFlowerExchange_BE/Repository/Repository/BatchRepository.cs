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
    public class BatchRepository : IBatchRepository
    {
        public async Task<List<Batch>> GetAllBatch()
        {
            var _context = new FlowerShopContext();
            var batches = await _context.Batches
               .Include(e => e.Company)   
               .Include(e => e.Flowers)
               .Where(e => e.Status == 0)
               .ToListAsync();
            return batches;
        }

        public async Task<Batch> GetBatchById(int id)
        {
            var _context = new FlowerShopContext();
            var existing = await _context.Batches.Include(e => e.Company).Include(e => e.Flowers).FirstOrDefaultAsync(b => b.BatchId == id);
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
                existing.EventName = batch.EventName;
                existing.EventDate = batch.EventDate;
                existing.BatchQuantity = batch.BatchQuantity;
                existing.RemainingQuantity = batch.RemainingQuantity;
                existing.Description = batch.Description;
                existing.EntryDate = batch.EntryDate;
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
            existing.Status = Status.Inactive;
            _context.Batches.Update(existing);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Batch>> GetAvailableBatchesByFlowerId(int flowerId)
        {
            using (var _context = new FlowerShopContext())
            {
                return await _context.Batches
                    .Include(b => b.Flowers) // Bao gồm các hoa trong lô
                    .Where(b => b.Flowers.Any(f => f.FlowerId == flowerId && f.RemainingQuantity > 0 && f.FlowerStatus == EnumList.FlowerStatus.Available)
                                 && b.RemainingQuantity > 0) // Kiểm tra số lượng còn lại của lô
                    .OrderBy(b => b.EntryDate) // Sắp xếp theo ngày nhập kho
                    .ToListAsync();
            }
        }


        public async Task UpdateBatch(Batch batch)
        {
            var _context = new FlowerShopContext();
            _context.Batches.Update(batch);
            await _context.SaveChangesAsync();
        }
    }
}
