using BusinessObject;
using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN231_2_EventFlowerExchange_FE.Service;
using System.Text.Json;

namespace PRN231_2_EventFlowerExchange_FE.Pages.FlowerPages
{
    public class EditFlowerModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly CloudinaryService _cloudinaryService;

        public EditFlowerModel(HttpClient httpClient, IConfiguration configuration, CloudinaryService cloudinaryService)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _cloudinaryService = cloudinaryService;
        }

        [BindProperty]
        public ListFlowerDTO Flower { get; set; }

        [BindProperty]
        public IFormFile ImageFile { get; set; } 

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var baseApiUrl = _configuration["ApiSettings:BaseUrl"];
            var response = await _httpClient.GetAsync($"{baseApiUrl}/api/flower/getby/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                Flower = JsonSerializer.Deserialize<ListFlowerDTO>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var baseApiUrl = _configuration["ApiSettings:BaseUrl"];

            // Fetch the existing flower to retain the current image if no new image is uploaded
            var existingFlowerResponse = await _httpClient.GetAsync($"{baseApiUrl}/api/flower/getby/{id}");

            if (!existingFlowerResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error fetching existing flower.");
                return Page();
            }

            var existingFlowerContent = await existingFlowerResponse.Content.ReadAsStringAsync();
            var existingFlower = JsonSerializer.Deserialize<ListFlowerDTO>(existingFlowerContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Only upload new image if a file is provided
            if (ImageFile != null && ImageFile.Length > 0)
            {
                Flower.Image = await _cloudinaryService.UploadImageAsync(ImageFile);
            }
            else
            {
                // If no new image is uploaded, retain the existing image
                Flower.Image = existingFlower.Image;
            }

            var jsonContent = JsonSerializer.Serialize(Flower);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{baseApiUrl}/api/flower/update/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/FlowerPages/FlowerDetailInBatch", new { batchId = Flower.BatchId });
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorMessage = JsonSerializer.Deserialize<ErrorResponse>(errorContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })?.Message ?? "Error updating flower.";

            ModelState.AddModelError(string.Empty, errorMessage);
            return Page();
        }


        public class ErrorResponse
        {
            public string Message { get; set; }
        }

    }
}
