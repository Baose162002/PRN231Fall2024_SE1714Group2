using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace PRN231_2_EventFlowerExchange_FE.Pages.UserPages
{
    public class UserIndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public UserIndexModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        public List<ListUserDTO> Users { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(token) || role != "Admin")
            {
                return RedirectToPage("/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"{_baseApiUrl}/user");

            if (response.IsSuccessStatusCode)
            {
                Users = await response.Content.ReadFromJsonAsync<List<ListUserDTO>>();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to load users.");
            }

            return Page();
        }
    }
}