using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessObject.DTO.Request;
using Microsoft.Extensions.Configuration;

namespace PRN231_2_EventFlowerExchange_FE.Pages.Register
{
    public class RegisterModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        [BindProperty]
        public CreateUserDTO RegisterRequest { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }

        public RegisterModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
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

            try
            {
                var jsonContent = new StringContent(JsonSerializer.Serialize(RegisterRequest), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseApiUrl}/api/user/register-buyer", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Registration successful! You can now log in.";
                    return RedirectToPage("/Index");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    var apiError = JsonSerializer.Deserialize<Dictionary<string, string>>(errorMessage);

                    if (apiError != null && apiError.TryGetValue("message", out var message))
                    {
                        ModelState.AddModelError(string.Empty, message);
                    }

                    return RedirectToPage("/Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
                return Page();
            }
        }
    }
}