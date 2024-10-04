﻿using Microsoft.AspNetCore.Mvc;
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

        public RegisterModel(HttpClient httpClient, IConfiguration configuration)
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

                // Gọi API Backend để đăng ký người dùng
                var response = await _httpClient.PostAsync($"{_baseApiUrl}/user/register-buyer", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    // Chuyển hướng đến trang login sau khi đăng ký thành công
                    return RedirectToPage("/Login/Login");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Error: {errorMessage}");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return Page();
            }
        }
    }
}