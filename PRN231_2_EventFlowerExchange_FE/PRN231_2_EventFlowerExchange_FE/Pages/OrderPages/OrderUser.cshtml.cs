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

        public List<ListOrderDTO> Orders { get; set; }
        public int UsersIds { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/api/order/user?userId={id}");

            if (response.IsSuccessStatusCode)
            {
                UsersIds = id;
                Orders = await response.Content.ReadFromJsonAsync<List<ListOrderDTO>>(); // Deserialize as a list of orders
            }
            else
            {
                return NotFound("Orders not found.");
            }

            return Page();
        }

        public string GetOrderStatusClass(string status)
        {
            return status switch
            {
                "Pending" => "bg-yellow-100 text-yellow-800",
                "Confirmed" => "bg-blue-100 text-blue-800",
                "Dispatched" => "bg-purple-100 text-purple-800",
                "Delivered" => "bg-green-100 text-green-800",
                _ => "bg-gray-100 text-gray-800"
            };
        }

    }
}