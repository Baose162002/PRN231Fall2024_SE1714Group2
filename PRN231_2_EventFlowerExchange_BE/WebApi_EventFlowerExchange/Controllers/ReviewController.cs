// File: WebApi_EventFlowerExchange/Controllers/ReviewController.cs
using BusinessObject.DTO.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Service.IService;
using System;
using System.Threading.Tasks;

namespace WebApi_EventFlowerExchange.Controllers
{
/*    [Authorize]
*/    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }


        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var reviews = await _reviewService.GetAllReviews();
            if (reviews == null || reviews.Count == 0)
            {
                return NotFound("No reviews found");
            }
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewById(int id)
        {
            var review = await _reviewService.GetReviewById(id);
            if (review == null)
            {
                return NotFound("Review not found");
            }
            return Ok(review);
        }

        [HttpGet("flower/{flowerId}")]
        public async Task<IActionResult> GetReviewsByFlowerId(int flowerId)
        {
            var reviews = await _reviewService.GetReviewsByFlowerId(flowerId); 
            if (reviews == null || reviews.Count == 0)
            {
                return NotFound("No reviews found for this flower");
            }
            return Ok(reviews);
        }


        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetReviewsByCustomerId(int customerId)
        {
            var reviews = await _reviewService.GetReviewsByCustomerId(customerId);
            if (reviews == null || reviews.Count == 0)
            {
                return NotFound("No reviews found for this customer");
            }
            return Ok(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDTO createReviewDTO)
        {
            try
            {
                await _reviewService.CreateReview(createReviewDTO);
                return Ok("Review created successfully");
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] UpdateReviewDTO updateReviewDTO)
        {
            try
            {
                await _reviewService.UpdateReview(id, updateReviewDTO);
                return Ok("Review updated successfully");
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                await _reviewService.DeleteReview(id);
                return Ok("Review deleted successfully");
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}