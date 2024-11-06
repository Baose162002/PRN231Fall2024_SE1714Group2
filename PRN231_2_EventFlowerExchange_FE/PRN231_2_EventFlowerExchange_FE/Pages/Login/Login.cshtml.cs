using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using BusinessObject.Dto.Request;
using BusinessObject.Dto.Response;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace PRN231_2_EventFlowerExchange_FE.Pages.Login
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public LoginUserRequest LoginRequest { get; set; }

        public LoginModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/auth/login", LoginRequest);

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var loginResult = await response.Content.ReadFromJsonAsync<UserResponseDto>(options);
                if (loginResult != null)
                {
                    HttpContext.Session.SetString("JWTToken", loginResult.Token);
                    HttpContext.Session.SetString("UserId", loginResult.UserId.ToString());
                    HttpContext.Session.SetString("UserRole", loginResult.Role);
                    HttpContext.Session.SetString("UserName", string.IsNullOrEmpty(loginResult.FullName) ? "User" : loginResult.FullName);

                    // Lưu CompanyId nếu người dùng là Seller
                    if (loginResult.Role == "Seller" && loginResult.CompanyId.HasValue)
                    {
                        HttpContext.Session.SetString("CompanyId", loginResult.CompanyId.Value.ToString());
                    }

                    if (loginResult.Role == "DeliveryPersonnel")
                    {
                        return RedirectToPage("/DeliveryPages/DeliveryIndex");
                    }

                    return RedirectToPage("/Index");
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var jsonDocument = JsonDocument.Parse(errorContent);
                ModelState.AddModelError(string.Empty, jsonDocument.RootElement.TryGetProperty("message", out var messageElement)
                    ? messageElement.GetString()
                    : "An unknown error occurred.");
            }

            return Page();
        }
    }
}
