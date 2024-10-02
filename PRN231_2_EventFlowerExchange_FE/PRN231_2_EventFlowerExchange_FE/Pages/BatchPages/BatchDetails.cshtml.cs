using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace PRN231_2_EventFlowerExchange_FE.Pages.BatchPages
{
    public class BatchDetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public BatchDetailsModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        public ListBatchDTO Batch { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Lấy token từ session
            var token = HttpContext.Session.GetString("JWTToken");
            Console.WriteLine(token);

            if (string.IsNullOrEmpty(token))
            {
                // Nếu không có token, redirect về trang đăng nhập
                return RedirectToPage("/Login");
            }

            // Thêm token vào header của HttpClient
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Gọi API lấy chi tiết batch
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/batch/{id}");

            if (response.IsSuccessStatusCode)
            {
                Batch = await response.Content.ReadFromJsonAsync<ListBatchDTO>();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Không thể tải chi tiết batch.");
            }

            return Page();
        }
    }
}
