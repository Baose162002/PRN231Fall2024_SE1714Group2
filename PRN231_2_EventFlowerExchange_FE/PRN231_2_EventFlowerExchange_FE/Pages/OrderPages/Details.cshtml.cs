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
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public DetailsModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public ListOrderDTO Order { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _clientFactory.CreateClient();

            var response = await client.GetAsync($"https://localhost:5011/api/order/{id}");

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
