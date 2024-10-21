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
using Repository.Repository;

namespace Service.Service
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IFlowerRepository _flowerRepository; 
        private readonly IBatchRepository _batchRepository; 

        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository, IFlowerRepository flowerRepository, IBatchRepository batchRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _flowerRepository = flowerRepository;
            _batchRepository = batchRepository;
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

        public async Task<List<ListReviewDTO>> GetReviewsByFlowerId(int flowerId)  
        {
            var reviews = await _reviewRepository.GetReviewsByFlowerId(flowerId);
            return _mapper.Map<List<ListReviewDTO>>(reviews);
        }

        public async Task<List<ListReviewDTO>> GetReviewsByCustomerId(int customerId)
        {
            var reviews = await _reviewRepository.GetReviewsByCustomerId(customerId);
            return _mapper.Map<List<ListReviewDTO>>(reviews);
        }

        public async Task CreateReview(CreateReviewDTO createReviewDTO)
        {
            // Lấy thông tin về loại hoa
            var flower = await _flowerRepository.GetFlowerById(createReviewDTO.FlowerId);
            if (flower == null)
            {
                throw new ArgumentException("Flower not found");
            }

            // Lấy thông tin batch liên quan đến hoa
            var batch = await _batchRepository.GetBatchById(flower.BatchId);
            if (batch == null)
            {
                throw new ArgumentException("Batch not found");
            }

            // Kiểm tra nếu user đang tạo review chính là người tạo ra batch này
            if (batch.CompanyId == createReviewDTO.CustomerId)
            {
                throw new ArgumentException("You cannot review a flower from your own batch");
            }

            // Tạo review 
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
