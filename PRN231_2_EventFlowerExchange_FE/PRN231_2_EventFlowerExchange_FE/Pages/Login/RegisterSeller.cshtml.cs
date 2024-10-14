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
    public class RegisterSellerModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        [BindProperty]
        public RegisterSellerDTO RegisterRequest { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }

        public RegisterSellerModel(HttpClient httpClient, IConfiguration configuration)
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
                var response = await _httpClient.PostAsync($"{_baseApiUrl}/api/user/register-seller", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Seller registration successful! You can now log in.";
                    return Page();
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    var apiError = JsonSerializer.Deserialize<Dictionary<string, string>>(errorMessage);

                    if (apiError != null && apiError.TryGetValue("message", out var message))
                    {
                        ModelState.AddModelError(string.Empty, message);
                    }

                    return Page();
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