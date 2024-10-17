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
    public class ReviewRepository : IReviewRepository
    {
        private readonly FlowerShopContext _context;

        public ReviewRepository()
        {
            _context = new FlowerShopContext();
        }

        public async Task<List<Review>> GetAllReviews()
        {
            return await _context.Reviews
                .ToListAsync();
        }

        public async Task<Review> GetReviewById(int id)
        {
            return await _context.Reviews
                                 .Include(r => r.Customer)  
                                 .FirstOrDefaultAsync(r => r.ReviewId == id);
        }


        public async Task<List<Review>> GetReviewsByFlowerId(int flowerId)  
        {
            return await _context.Reviews
                .Include(r => r.Customer)
                .Where(r => r.FlowerId == flowerId)  
                .ToListAsync();
        }

        public async Task<List<Review>> GetReviewsByCustomerId(int customerId)
        {
            return await _context.Reviews
                                 .Include(r => r.Customer)  
                                 .Where(r => r.CustomerId == customerId)
                                 .ToListAsync();
        }


        public async Task Create(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }
    }
}
