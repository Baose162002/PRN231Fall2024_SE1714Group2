using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessObject;
using BusinessObject.DTO.Response;
using System.Text.Json;

namespace PRN231_2_EventFlowerExchange_FE.Pages.CompanyPages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;
        public IndexModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        public IList<CompanyDTO> Company { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/api/Company");

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                Company = await response.Content.ReadFromJsonAsync<List<CompanyDTO>>(options);
            }
        }
    }
}
