using BusinessObject.DTO.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace PRN231_2_EventFlowerExchange_FE.Pages.FlowerPages
{
    public class CreateFlowerModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public CreateFlowerModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        [BindProperty]
        public CreateFlowerDTO FlowerInput { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var json = JsonSerializer.Serialize(FlowerInput);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/flower";
            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var flowerId = await response.Content.ReadAsStringAsync();
                return RedirectToPage("/BatchPages/CreateBatch", new { flowerId });
            }

            // Handle errors...
            return Page();
        }
    }

}
