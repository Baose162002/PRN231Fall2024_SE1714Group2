using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PRN231_2_EventFlowerExchange_FE.Pages.FlowerPages
{
    public class FlowerDetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public FlowerDetailsModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        public ListFlowerDTO Flower { get; set; }

        public async Task OnGetAsync(int id)
        {
            // Lấy token từ session (nếu cần)
            var token = HttpContext.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Gọi API để lấy chi tiết hoa theo id
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/Flower/{id}");

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                Flower = await response.Content.ReadFromJsonAsync<ListFlowerDTO>(options);
            }
            else
            {
                Flower = null;
                ModelState.AddModelError(string.Empty, "Không thể tải chi tiết hoa.");
            }
        }
    }
}
