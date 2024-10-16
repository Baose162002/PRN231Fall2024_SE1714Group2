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
            // Gọi API
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/api/order");

            if (response.IsSuccessStatusCode)
            {
                Orders = await response.Content.ReadFromJsonAsync<List<ListOrderDTO>>();
            }
            else
            {
                return NotFound("Unable to retrieve orders");
            }

            return Page();
        }
    }

}
