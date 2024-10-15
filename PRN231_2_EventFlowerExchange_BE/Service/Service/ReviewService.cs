using AutoMapper;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using BusinessObject;
using Repository.IRepository;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Enum;

namespace Service.Service
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        public async Task<List<Review>> GetAllReviews()
        {
            var reviews = await _reviewRepository.GetAllReviews();
            return _mapper.Map<List<Review>>(reviews);
        }

        public async Task<ListReviewDTO> GetReviewById(int id)
        {
            var review = await _reviewRepository.GetReviewById(id);
            return _mapper.Map<ListReviewDTO>(review);
        }

        public async Task<List<ListReviewDTO>> GetReviewsByBatchId(int batchId)
        {
            var reviews = await _reviewRepository.GetReviewsByBatchId(batchId);
            return _mapper.Map<List<ListReviewDTO>>(reviews);
        }

        public async Task<List<ListReviewDTO>> GetReviewsByCustomerId(int customerId)
        {
            var reviews = await _reviewRepository.GetReviewsByCustomerId(customerId);
            return _mapper.Map<List<ListReviewDTO>>(reviews);
        }

        public async Task CreateReview(CreateReviewDTO createReviewDTO)
        {
            var review = _mapper.Map<Review>(createReviewDTO);
            review.ReviewDate = DateTime.Now;
            review.Status = EnumList.Status.Active;
            await _reviewRepository.Create(review);
        }

        public async Task UpdateReview(int id, UpdateReviewDTO updateReviewDTO)
        {
            var existingReview = await _reviewRepository.GetReviewById(id);
            if (existingReview == null)
            {
                throw new ArgumentException("Review not found");
            }

            _mapper.Map(updateReviewDTO, existingReview);
            await _reviewRepository.Update(existingReview);
        }

        public async Task DeleteReview(int id)
        {
            await _reviewRepository.Delete(id);
        }
    }
}
