using BusinessObject;
using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PRN231_2_EventFlowerExchange_FE.Pages.FlowerPages
{
    public class FlowerDetailInBatchModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public FlowerDetailInBatchModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public List<ListFlowerDTO> Flowers { get; set; }

        public async Task OnGetAsync(int batchId)
        {
            // Lấy BaseUrl từ appsettings.json
            var baseUrl = _configuration.GetValue<string>("ApiSettings:BaseUrl");

            // Gọi API OData để lấy danh sách hoa theo BatchId
            var response = await _httpClient.GetAsync($"{baseUrl}/odata/Flower?$filter=BatchId eq {batchId}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var flowerData = JsonDocument.Parse(jsonString).RootElement.GetProperty("value");

                Flowers = JsonSerializer.Deserialize<List<ListFlowerDTO>>(flowerData.ToString(), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }
    }
}
