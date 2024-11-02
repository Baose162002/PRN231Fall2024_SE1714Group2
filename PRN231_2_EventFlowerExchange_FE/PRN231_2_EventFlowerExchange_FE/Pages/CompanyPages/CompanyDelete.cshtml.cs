using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessObject.DTO.Response;
using System.Net.Http.Headers;
using System.Net;
using System.Text.Json;

namespace PRN231_2_EventFlowerExchange_FE.Pages.CompanyPages
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public DeleteModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        [BindProperty]
        public CompanyDTO Company { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/api/Company/{id}");

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                Company = await response.Content.ReadFromJsonAsync<CompanyDTO>(options);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ModelState.AddModelError(string.Empty, "Token is invalid or has expired.");
                return RedirectToPage("/Login/Login");
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"Error loading company details: {response.ReasonPhrase}");
                return RedirectToPage("/CompanyPages/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"{_baseApiUrl}/api/Company/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/CompanyPages/CompanyIndex");
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ModelState.AddModelError(string.Empty, "Token is invalid or has expired.");
                return RedirectToPage("/Login/Login");
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"Error deleting company: {response.ReasonPhrase}");
                return Page();
            }
        }
    }
}