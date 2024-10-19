using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace PRN231_2_EventFlowerExchange_FE.Pages.CompanyPages
{
    public class CompanyDetailFromUserModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;
        public CompanyDetailFromUserModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        public CompanyDTO Company { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Gọi API để lấy chi tiết hoa theo id
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/api/company/user/{id}");

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                Company = await response.Content.ReadFromJsonAsync<CompanyDTO>(options);
            }
            return Page();
        }
    }
}
