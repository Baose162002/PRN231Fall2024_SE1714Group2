using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using BusinessObject.Dto.Request;
using BusinessObject.Dto.Response;
using System.Text.Json;

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
            var response = await client.PostAsJsonAsync($"{_configuration["ApiSettings:BaseUrl"]}/auth/login", LoginRequest);

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var loginResult = await response.Content.ReadFromJsonAsync<UserResponseDto>(options);
                if (loginResult != null)
                {
                    if (!string.IsNullOrEmpty(loginResult.Token))
                    {
                        HttpContext.Session.SetString("JWTToken", loginResult.Token);
                    }

                    if (!string.IsNullOrEmpty(loginResult.Role))
                    {
                        HttpContext.Session.SetString("UserRole", loginResult.Role);
                    }

                    if (!string.IsNullOrEmpty(loginResult.FullName))
                    {
                        HttpContext.Session.SetString("UserName", loginResult.FullName);
                    }
                    else
                    {
                        HttpContext.Session.SetString("UserName", "User"); // Default value if FullName is null
                    }

                    return RedirectToPage("/BatchPages/BatchIndex");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
}