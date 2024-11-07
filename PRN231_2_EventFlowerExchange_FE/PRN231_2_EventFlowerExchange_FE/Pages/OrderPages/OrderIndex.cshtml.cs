using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObject;
using Newtonsoft.Json;
using BusinessObject.DTO.Response;
using System.Net.Http.Headers;
using System.Net;

namespace PRN231_2_EventFlowerExchange_FE.Pages.OrderPages
{
    public class OrderIndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public OrderIndexModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }


        public List<ListOrderDTO> Orders { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            //// Lấy token từ session
            //var token = HttpContext.Session.GetString("JWTToken");


            //if (string.IsNullOrEmpty(token))
            //{
            //    // Nếu không có token, redirect về trang đăng nhập
            //    return RedirectToPage("/Login/Login");
            //}

            //// Thêm token vào header của HttpClient
            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Gọi API
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/api/order");

            if (response.IsSuccessStatusCode)
            {
                Orders = await response.Content.ReadFromJsonAsync<List<ListOrderDTO>>();
            }
            //else if (response.StatusCode == HttpStatusCode.Unauthorized)
            //{
            //    // Kiểm tra nếu token bị hết hạn hoặc không hợp lệ
            //    ModelState.AddModelError(string.Empty, "Token không hợp lệ hoặc hết hạn.");
            //    return RedirectToPage("/Login");
            //}
            else
            {
                ModelState.AddModelError(string.Empty, $"Lỗi khi tải danh sách order: {response.ReasonPhrase}");
            }

            return Page();
        }
    }

}