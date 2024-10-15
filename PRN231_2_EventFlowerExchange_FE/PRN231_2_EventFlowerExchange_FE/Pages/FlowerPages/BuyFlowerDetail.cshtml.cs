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

        public async Task<IActionResult> OnGetAsync(int id)
        {
         /*   var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }*/

            // Thiết lập header Authorization với JWT Token
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer");
            var baseApiUrl = _configuration["ApiSettings:BaseUrl"];

            // Gọi API để lấy chi tiết của hoa
            var response = await _httpClient.GetAsync($"{baseApiUrl}/api/flower/getby/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                Flower = JsonSerializer.Deserialize<ListFlowerDTO>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to load flower data.");
            }

            return Page();
        }
    }
}
