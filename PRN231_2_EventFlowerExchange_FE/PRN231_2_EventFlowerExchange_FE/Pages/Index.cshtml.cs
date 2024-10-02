﻿using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PRN231_2_EventFlowerExchange_FE.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public IndexModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        public List<ListFlowerDTO> Flowers { get; set; }

        public async Task OnGetAsync()
        {
            // Lấy token từ session (nếu cần)
            var token = HttpContext.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Gọi API để lấy danh sách hoa
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/Flower");

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                Flowers = await response.Content.ReadFromJsonAsync<List<ListFlowerDTO>>(options);
            }
            else
            {
                Flowers = new List<ListFlowerDTO>();
                ModelState.AddModelError(string.Empty, "Không thể tải danh sách hoa.");
            }
        }
    }
}
