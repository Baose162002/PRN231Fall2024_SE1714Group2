using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace PRN231_2_EventFlowerExchange_FE.Pages.FlowerPages
{
    public class EditFlowerModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public EditFlowerModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        [BindProperty]
        public ListFlowerDTO Flower { get; set; }

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

        public async Task<IActionResult> OnPostAsync()
        {
            var baseApiUrl = _configuration["ApiSettings:BaseUrl"];
            var jsonContent = JsonSerializer.Serialize(Flower);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{baseApiUrl}/api/flower/update", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/FlowerPages/FlowerDetailInBatch");
            }

            ModelState.AddModelError(string.Empty, "Error updating flower.");
            return Page();
        }
    }
}
