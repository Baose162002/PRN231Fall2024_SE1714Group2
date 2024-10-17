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



    }
}