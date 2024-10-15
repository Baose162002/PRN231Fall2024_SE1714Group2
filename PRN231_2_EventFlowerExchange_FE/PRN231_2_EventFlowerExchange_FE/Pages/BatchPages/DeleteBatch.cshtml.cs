using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PRN231_2_EventFlowerExchange_FE.Pages.BatchPages
{
    public class DeleteBatchModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public DeleteBatchModel(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        [BindProperty]
        public ListBatchDTO Batch { get; set; } // This holds the data for the Batch to be deleted.

        public string BaseApiUrl { get; private set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login/Login");
            }

            // Thiết lập Header Authorization với JWT Token
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            BaseApiUrl = _configuration["ApiSettings:BaseUrl"];

            // Lấy thông tin batch từ API
            var response = await _httpClient.GetAsync($"{BaseApiUrl}/api/batch/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                Batch = JsonSerializer.Deserialize<ListBatchDTO>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to load batch data.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            BaseApiUrl = _configuration["ApiSettings:BaseUrl"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"{BaseApiUrl}/api/batch/{id}"); // Put request to update status.

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("./BatchIndex");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to delete the batch.");
                return Page();
            }
        }
    }
}
