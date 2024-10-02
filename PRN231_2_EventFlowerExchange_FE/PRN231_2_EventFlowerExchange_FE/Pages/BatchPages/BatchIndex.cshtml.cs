using BusinessObject;
using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Net.Http.Headers;

namespace PRN231_2_EventFlowerExchange_FE.Pages.BatchPages
{
    public class BatchIndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public BatchIndexModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        public List<ListBatchDTO> Batches { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Lấy token từ session
            var token = HttpContext.Session.GetString("JWTToken");
            

            if (string.IsNullOrEmpty(token))
            {
                // Nếu không có token, redirect về trang đăng nhập
                return RedirectToPage("/Login/Login");
            }

            // Thêm token vào header của HttpClient
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Gọi API
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/batch");

            if (response.IsSuccessStatusCode)
            {
                Batches = await response.Content.ReadFromJsonAsync<List<ListBatchDTO>>();
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Kiểm tra nếu token bị hết hạn hoặc không hợp lệ
                ModelState.AddModelError(string.Empty, "Token không hợp lệ hoặc hết hạn.");
                return RedirectToPage("/Login");
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"Lỗi khi tải danh sách batch: {response.ReasonPhrase}");
            }

            return Page();
        }
    }

}
