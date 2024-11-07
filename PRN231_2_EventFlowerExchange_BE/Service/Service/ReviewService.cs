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
        private readonly ICompanyRepository _companyRepository; 

        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository, IFlowerRepository flowerRepository, IBatchRepository batchRepository, ICompanyRepository companyRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _flowerRepository = flowerRepository;
            _batchRepository = batchRepository;
            _companyRepository = companyRepository;
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
            // Retrieve the flower based on the provided FlowerId
            var flower = await _flowerRepository.GetFlowerById(createReviewDTO.FlowerId);
            if (flower == null)
            {
                throw new ArgumentException("Flower not found");
            }

            // Retrieve the batch associated with the flower
            var batch = await _batchRepository.GetBatchById(flower.BatchId);
            if (batch == null)
            {
                throw new ArgumentException("Batch not found");
            }

            // Retrieve the company associated with the batch
            var company = await _companyRepository.GetCompanyByID(batch.CompanyId);
            if (company == null)
            {
                throw new ArgumentException("Company not found");
            }

            // Check if the user creating the review is the owner of the company
            if (company.UserId == createReviewDTO.CustomerId)
            {
                throw new ArgumentException("You cannot review a flower from your own batch");
            }

            // Create the review
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
