using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetAllReviews();
        Task<Review> GetReviewById(int id);
        Task<List<Review>> GetReviewsByFlowerId(int flowerId);
        Task<List<Review>> GetReviewsByCustomerId(int customerId);
        Task Create(Review review);
        Task Update(Review review);
        Task Delete(int id);
    }
}