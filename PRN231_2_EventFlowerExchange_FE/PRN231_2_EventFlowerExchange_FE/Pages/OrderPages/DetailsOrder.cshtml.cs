﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessObject.DTO.Response;
using Newtonsoft.Json;

namespace PRN231_2_EventFlowerExchange_FE.Pages.OrderPages
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public DetailsModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        public ListOrderDTO Order { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Gọi API để lấy thông tin chi tiết của đơn hàng
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/api/order/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                Order = JsonConvert.DeserializeObject<ListOrderDTO>(jsonResponse);
            }
            else
            {
                return NotFound("Order not found");
            }

            return Page();
        }
    }
}
