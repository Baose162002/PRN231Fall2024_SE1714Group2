﻿using BusinessObject;
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
               .ToListAsync();
            return batches;
        }

        public async Task<Batch> GetBatchById(int id)
        {
            var _context = new FlowerShopContext();
            var existing = await _context.Batches.Include(e => e.Company).FirstOrDefaultAsync(b => b.BatchId == id);
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
            using (var _context = new FlowerShopContext())
            {
                var existing = await GetBatchById(id);
                if (existing != null)
                {
                    existing.BatchName = batch.BatchName;
                    existing.EventName = batch.EventName;
                    existing.EventDate = batch.EventDate;
                    existing.BatchQuantity = batch.BatchQuantity;
                    existing.RemainingQuantity = batch.RemainingQuantity;
                    existing.Description = batch.Description;
                    existing.EntryDate = batch.EntryDate;
                    existing.Status = EnumList.Status.Active;

                    var flowersInBatch = await _context.Flowers.Where(f => f.BatchId == existing.BatchId).ToListAsync();

                    if (flowersInBatch.Any())
                    {
                        foreach (var flower in flowersInBatch)
                        {
                            flower.Status = EnumList.Status.Active;
                        }

                        _context.Flowers.UpdateRange(flowersInBatch);
                    }

                    _context.Batches.Update(existing);
                    await _context.SaveChangesAsync();
                }
            }
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

        public async Task<List<Flower>> GetFlowersBySimilarTypeAndColorAndEarliestBatch(int flowerId)
        {
            using (var _context = new FlowerShopContext())
            {
                // Bước 1: Lấy thông tin của Flower ban đầu
                var initialFlower = await _context.Flowers
                    .FirstOrDefaultAsync(f => f.FlowerId == flowerId);

                if (initialFlower == null)
                {
                    return new List<Flower>(); // Không tìm thấy Flower với flowerId
                }

                var flowerType = initialFlower.Type; // Lấy Type của Flower
                var flowerColor = initialFlower.Color; // Lấy Color của Flower

                // Bước 2: Lấy tất cả các Batch chứa Flower có Type và Color tương tự
                var similarFlowers = await _context.Flowers
                    .Include(f => f.Batch) // Bao gồm thông tin Batch
                    .Where(f => f.Type == flowerType &&
                                 f.Color == flowerColor &&
                                 f.RemainingQuantity > 0 &&
                                 f.FlowerStatus == EnumList.FlowerStatus.Available &&
                                 f.Condition == EnumList.FlowerCondition.Fresh)
                    .ToListAsync();

                // Bước 3: Lấy Flower với EntryDate nhỏ nhất theo Type và Color
                var earliestBatchFlowers = similarFlowers
                    .GroupBy(f => new { f.Type, f.Color }) // Nhóm theo Type và Color
                    .Select(g => g
                        .OrderBy(f => f.Batch.EntryDate) // Sắp xếp theo EntryDate
                        .FirstOrDefault()) // Lấy Flower có EntryDate nhỏ nhất trong nhóm
                    .Where(f => f != null) // Lọc bỏ các giá trị null
                    .ToList();


                return earliestBatchFlowers;
            }
        }

        public async Task<List<Flower>> GetFlowersBySimilarTypeAndColorAndEarliestBatchAndCompany(int flowerId)
        {
            using (var _context = new FlowerShopContext())
            {
                // Bước 1: Lấy thông tin của Flower ban đầu
                var initialFlower = await _context.Flowers
                    .Include(f => f.Batch) // Bao gồm Batch để truy cập CompanyId
                    .FirstOrDefaultAsync(f => f.FlowerId == flowerId);

                if (initialFlower == null)
                {
                    return new List<Flower>(); // Không tìm thấy Flower với flowerId
                }

                var flowerType = initialFlower.Type; // Lấy Type của Flower
                var flowerColor = initialFlower.Color; // Lấy Color của Flower
                var companyId = initialFlower.Batch.CompanyId; // Lấy CompanyId từ Batch của Flower

                // Bước 2: Lấy tất cả các Batch chứa Flower có Type, Color và cùng CompanyId
                var similarFlowers = await _context.Flowers
                    .Include(f => f.Batch) // Bao gồm thông tin Batch
                    .Where(f => f.Type == flowerType &&
                                 f.Color == flowerColor &&
                                 f.RemainingQuantity > 0 &&
                                 f.FlowerStatus == EnumList.FlowerStatus.Available &&
                                 f.Condition == EnumList.FlowerCondition.Fresh &&
                                 f.Batch.CompanyId == companyId) // Chỉ lấy các Batch của cùng Company
                    .ToListAsync();

                // Bước 3: Lấy Flower với EntryDate nhỏ nhất theo Type và Color trong cùng Company
                var earliestBatchFlowers = similarFlowers
                    .GroupBy(f => new { f.Type, f.Color }) // Nhóm theo Type và Color
                    .Select(g => g
                        .OrderBy(f => f.Batch.EntryDate) // Sắp xếp theo EntryDate
                        .FirstOrDefault()) // Lấy Flower có EntryDate nhỏ nhất trong nhóm
                    .Where(f => f != null) // Lọc bỏ các giá trị null
                    .ToList();

                return earliestBatchFlowers;
            }
        }
        public async Task CheckAndUpdateBatchStatus()
        {
            using (var _context = new FlowerShopContext())
            {
                var batches = await _context.Batches.ToListAsync();

                foreach (var batch in batches)
                {
                    if ((DateTime.Now - batch.EntryDate).TotalDays > 1)
                    {
                        var flowersInBatch = await _context.Flowers.Where(f => f.BatchId == batch.BatchId).ToListAsync();

                        bool hasAvailableFlowers = flowersInBatch.Any(f => f.RemainingQuantity > 0);
                        bool batchHasRemaining = batch.RemainingQuantity > 0;

                        if (batchHasRemaining && hasAvailableFlowers)
                        {
                            batch.Status = Status.NeedsReview;

                            foreach (var flower in flowersInBatch)
                            {
                                if (flower.RemainingQuantity > 0)
                                {
                                    flower.Status = Status.NeedsReview;
                                }
                            }

                            _context.Batches.Update(batch);
                            _context.Flowers.UpdateRange(flowersInBatch);
                        }
                        else if (batchHasRemaining && !hasAvailableFlowers)
                        {
                            batch.Status = Status.NeedsReview;

                            foreach (var flower in flowersInBatch)
                            {
                                flower.Status = Status.NeedsReview;
                            }

                            _context.Batches.Update(batch);
                            _context.Flowers.UpdateRange(flowersInBatch);
                        }
                    }
                }

                await _context.SaveChangesAsync();
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
