using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessObject.DTO.Response;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace PRN231_2_EventFlowerExchange_FE.Pages.CompanyPages
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;
        public DetailsModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        public CompanyDTO Company { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Gọi API để lấy chi tiết hoa theo id
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/api/Company/{id}");

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                Company = await response.Content.ReadFromJsonAsync<CompanyDTO>(options);
            }
            return Page();
        }
    }
}
