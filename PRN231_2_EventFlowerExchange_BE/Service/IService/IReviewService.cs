using BusinessObject;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IReviewService
    {
        Task<List<Review>> GetAllReviews();
        Task<ListReviewDTO> GetReviewById(int id);
        Task<List<ListReviewDTO>> GetReviewsByBatchId(int batchId);
        Task<List<ListReviewDTO>> GetReviewsByCustomerId(int customerId);
        Task CreateReview(CreateReviewDTO createReviewDTO);
        Task UpdateReview(int id, UpdateReviewDTO updateReviewDTO);
        Task DeleteReview(int id);
    }
}
