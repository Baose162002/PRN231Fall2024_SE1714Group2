using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObject;
using System.Configuration;
using System.Text.Json.Serialization;
using System.Text.Json;
using BusinessObject.DTO.Response;
using System.Net.Http.Headers;

namespace PRN231_2_EventFlowerExchange_FE.Pages.DeliveryPages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public IndexModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public IList<ListOrderForDeliveryDTO> Deliveries { get; set; } = new List<ListOrderForDeliveryDTO>();

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(token) || userRole != "DeliveryPersonnel")
            {
                ModelState.AddModelError(string.Empty, "Unauthorized access. Only Delivery Personnel can view this page.");
                return RedirectToPage("/Login/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/api/Delivery/order";
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                Deliveries = await response.Content.ReadFromJsonAsync<List<ListOrderForDeliveryDTO>>(options);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error fetching delivery data: {errorContent}");
            }

            return Page();
        }
    }
}
