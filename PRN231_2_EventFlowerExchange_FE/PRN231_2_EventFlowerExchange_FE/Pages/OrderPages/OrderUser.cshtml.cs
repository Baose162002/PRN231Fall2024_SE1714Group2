using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessObject.DTO.Response;

namespace PRN231_2_EventFlowerExchange_FE.Pages.OrderPages
{
    public class OrderUser : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public OrderUser(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        public ListOrderDTO Order { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/api/order/{id}");
            if (response.IsSuccessStatusCode)
            {
                Order = await response.Content.ReadFromJsonAsync<ListOrderDTO>();
            }
            else
            {
                return NotFound("Order not found.");
            }

            return Page();
        }
    }
}