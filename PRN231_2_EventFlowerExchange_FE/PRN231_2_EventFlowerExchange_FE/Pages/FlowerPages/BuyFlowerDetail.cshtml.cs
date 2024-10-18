using BusinessObject;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PRN231_2_EventFlowerExchange_FE.Pages.FlowerPages
{
    public class BuyFlowerDetailModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public BuyFlowerDetailModel(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public ListFlowerDTO Flower { get; set; }
        public List<ListReviewDTO> Reviews { get; set; }

        [BindProperty]
        public CreateReviewDTO NewReview { get; set; }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer");
            var baseApiUrl = _configuration["ApiSettings:BaseUrl"];

            // Fetch flower details
            var flowerResponse = await _httpClient.GetAsync($"{baseApiUrl}/api/flower/getby/{id}");
            if (flowerResponse.IsSuccessStatusCode)
            {
                var jsonContent = await flowerResponse.Content.ReadAsStringAsync();
                Flower = JsonSerializer.Deserialize<ListFlowerDTO>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to load flower data.");
                return Page();
            }

            // Only fetch reviews if we successfully got the flower
            if (Flower != null)
            {
                // Fetch reviews
                var reviewResponse = await _httpClient.GetAsync($"{baseApiUrl}/api/review/flower/{id}");
                if (reviewResponse.IsSuccessStatusCode)
                {
                    var jsonContent = await reviewResponse.Content.ReadAsStringAsync();
                    Reviews = JsonSerializer.Deserialize<List<ListReviewDTO>>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to load review data.");
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSubmitReviewAsync(int id) 
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Fetch the flower details again for the POST request
            var baseApiUrl = _configuration["ApiSettings:BaseUrl"];
            var flowerResponse = await _httpClient.GetAsync($"{baseApiUrl}/api/flower/getby/{id}");
            if (flowerResponse.IsSuccessStatusCode)
            {
                var jsonContent = await flowerResponse.Content.ReadAsStringAsync();
                Flower = JsonSerializer.Deserialize<ListFlowerDTO>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to load flower data.");
                return Page();
            }

            // Set the flower ID and customer ID
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                ModelState.AddModelError(string.Empty, "You must be logged in to submit a review.");
                return Page();
            }

            NewReview.CustomerId = int.Parse(userIdString);
            NewReview.FlowerId = Flower.FlowerId;

            // Send the review to the backend API
            var jsonContentReview = JsonSerializer.Serialize(NewReview);
            var content = new StringContent(jsonContentReview, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{baseApiUrl}/api/review", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage(new { id = Flower.FlowerId });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to submit review.");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostUpdateReviewAsync(int reviewId, int flowerId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var baseApiUrl = _configuration["ApiSettings:BaseUrl"];

            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                ModelState.AddModelError(string.Empty, "You must be logged in to update a review.");
                return Page();
            }

            var updateReview = new UpdateReviewDTO
            {
                Rating = NewReview.Rating,
                Feedback = NewReview.Feedback,
                ReviewDate = DateTime.Now
            };

            var jsonContentReview = JsonSerializer.Serialize(updateReview);
            var content = new StringContent(jsonContentReview, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{baseApiUrl}/api/review/{reviewId}", content);

            if (response.IsSuccessStatusCode)
            {
                // Chuyển hướng sau khi cập nhật thành công
                return RedirectToPage(new { id = flowerId });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to update review.");
                return Page();
            }
        }


        public async Task<IActionResult> OnPostDeleteReviewAsync(int reviewId, int flowerId)
        {
            var baseApiUrl = _configuration["ApiSettings:BaseUrl"];

            // Xóa review
            var response = await _httpClient.DeleteAsync($"{baseApiUrl}/api/review/{reviewId}");

            if (response.IsSuccessStatusCode)
            {
                // Lấy lại thông tin của Flower để đảm bảo không bị null
                var flowerResponse = await _httpClient.GetAsync($"{baseApiUrl}/api/flower/getby/{flowerId}");
                if (flowerResponse.IsSuccessStatusCode)
                {
                    var jsonContent = await flowerResponse.Content.ReadAsStringAsync();
                    Flower = JsonSerializer.Deserialize<ListFlowerDTO>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                return RedirectToPage(new { id = flowerId });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to delete review.");
                return Page();
            }
        }




    }
}