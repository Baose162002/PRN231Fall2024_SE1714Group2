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

        public RegisterSellerModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];  // Lấy BaseUrl từ appsettings.json
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
                // Tạo nội dung JSON từ RegisterRequest
                var jsonContent = new StringContent(JsonSerializer.Serialize(RegisterRequest), Encoding.UTF8, "application/json");

                // Gọi API Backend để đăng ký người bán
                var response = await _httpClient.PostAsync($"{_baseApiUrl}/user/register-seller", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    // Chuyển hướng đến trang login sau khi đăng ký thành công
                    return RedirectToPage("/Login/Login");
                }
                else
                {
                    // Đọc thông báo lỗi từ phản hồi
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    var apiError = JsonSerializer.Deserialize<Dictionary<string, string>>(errorMessage);

                    // Kiểm tra xem có thông báo lỗi cụ thể không
                    if (apiError != null && apiError.TryGetValue("message", out var message))
                    {
                        // Thêm thông báo lỗi cụ thể vào ModelState
                        ModelState.AddModelError(string.Empty, message);
                    }

                    return Page();
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu cần
                return Page();
            }
        }
    }
}
