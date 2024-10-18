using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace PRN231_2_EventFlowerExchange_FE.Pages.FlowerPages
{
    public class DeleteFlowerModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public DeleteFlowerModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public ListFlowerDTO Flower { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var baseApiUrl = _configuration["ApiSettings:BaseUrl"];
            var response = await _httpClient.GetAsync($"{baseApiUrl}/api/flower/getby/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                Flower = JsonSerializer.Deserialize<ListFlowerDTO>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var baseApiUrl = _configuration["ApiSettings:BaseUrl"];
            var existingFlowerResponse = await _httpClient.GetAsync($"{baseApiUrl}/api/flower/getby/{id}");

            if (!existingFlowerResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error fetching existing flower.");
                return Page();
            }

            var existingFlowerContent = await existingFlowerResponse.Content.ReadAsStringAsync();
            var existingFlower = JsonSerializer.Deserialize<ListFlowerDTO>(existingFlowerContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var response = await _httpClient.DeleteAsync($"{baseApiUrl}/api/flower/delete/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/FlowerPages/FlowerDetailInBatch", new { batchId = existingFlower.BatchId });
            }

            ModelState.AddModelError(string.Empty, "Error deleting flower.");
            return Page();
        }

    }
}
