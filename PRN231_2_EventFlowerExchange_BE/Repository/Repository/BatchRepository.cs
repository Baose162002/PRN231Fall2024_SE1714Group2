using BusinessObject;
using BusinessObject.DTO.Request;
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
        public async Task CreateBatchAndFlowerAsync(CreateBatchAndFlowerDTO batchAndFlower)
        {
            var _context = new FlowerShopContext();
            var batch = new Batch
            {
                BatchName = batchAndFlower.BatchName,
                EventName = batchAndFlower.EventName,
                EventDate = batchAndFlower.EventDate,
                BatchQuantity = 0, // You can calculate later based on Flower quantity
                RemainingQuantity = 0, // Calculate based on Flower quantity
                Description = batchAndFlower.Description,
                EntryDate = batchAndFlower.EntryDate,
                CompanyId = batchAndFlower.CompanyId,
                Status = EnumList.Status.Active
            };

            _context.Batches.Add(batch);
            await _context.SaveChangesAsync(); // Save the batch to get BatchId

            // Group flowers by Type, Color, and Origin, summing RemainingQuantity for duplicates
            var groupedFlowers = batchAndFlower.Flowers
                .GroupBy(f => new { f.Type, f.Color, f.Origin })
                .Select(g => new
                {
                    Type = g.Key.Type,
                    Color = g.Key.Color,
                    Origin = g.Key.Origin,
                    Name = g.First().Name, // Take the first name (assuming it’s the same for duplicates)
                    Image = g.First().Image, // Take the first image (or handle images differently if needed)
                    PricePerUnit = g.First().PricePerUnit, // Take the first price (assuming it’s the same for duplicates)
                    Description = g.First().Description, // Take the first description (assuming it’s the same for duplicates)
                    TotalQuantity = g.Sum(f => f.RemainingQuantity) // Sum RemainingQuantity
                });

            // Create flower entities for each unique combination of Type, Color, and Origin
            foreach (var flower in groupedFlowers)
            {
                var flowerEntity = new Flower
                {
                    BatchId = batch.BatchId,
                    Name = flower.Name,
                    Type = flower.Type,
                    Color = flower.Color,
                    Origin = flower.Origin,
                    Image = flower.Image,
                    PricePerUnit = flower.PricePerUnit,
                    RemainingQuantity = flower.TotalQuantity,
                    Description = flower.Description,
                    FlowerStatus = EnumList.FlowerStatus.Available,
                    Status = EnumList.Status.Active,
                    Condition = EnumList.FlowerCondition.Fresh
                };
                _context.Flowers.Add(flowerEntity);
            }

            await _context.SaveChangesAsync(); // Save flowers after batch
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
                    existing.BatchQuantity = 0;
                    existing.RemainingQuantity = 0;
                    existing.Description = batch.Description;
                    existing.EntryDate = batch.EntryDate;
                    existing.Status = EnumList.Status.Active;
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
                    var flowersInBatch = await _context.Flowers.Where(f => f.BatchId == batch.BatchId).ToListAsync();

                    // Kiểm tra nếu batch đã quá hạn (lớn hơn 1 ngày)
                    if ((DateTime.Now - batch.EntryDate).TotalDays > 4)
                    {
                        bool hasAvailableFlowers = flowersInBatch.Any(f => f.RemainingQuantity > 0);

                        if (hasAvailableFlowers)
                        {
                            batch.Status = Status.Overdue; // Batch đã quá hạn

                            foreach (var flower in flowersInBatch)
                            {
                                if (flower.RemainingQuantity > 0)
                                {
                                    flower.Status = Status.Overdue;
                                }
                            }
                        }
                        else
                        {
                            batch.Status = Status.Overdue;

                            foreach (var flower in flowersInBatch)
                            {
                                flower.Status = Status.Overdue;
                            }
                        }

                        _context.Batches.Update(batch);
                    }

                    foreach (var flower in flowersInBatch)
                    {
                        if (flower.RemainingQuantity == 0)
                        {
                            flower.FlowerStatus = FlowerStatus.SoldOut;
                            flower.Status = Status.Inactive;
                        }
                    }

                    _context.Flowers.UpdateRange(flowersInBatch);
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
