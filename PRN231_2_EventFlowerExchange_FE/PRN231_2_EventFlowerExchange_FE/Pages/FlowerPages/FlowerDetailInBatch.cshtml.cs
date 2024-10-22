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
        public int BatchId { get; set; }
        public ListBatchDTO Batch { get; set; }

        public async Task OnGetAsync(int batchId)
        {
            // Set the BatchId
            BatchId = batchId;

            // Lấy BaseUrl từ appsettings.json
            var baseUrl = _configuration.GetValue<string>("ApiSettings:BaseUrl");

            // Gọi API để lấy thông tin Batch
            var batchResponse = await _httpClient.GetAsync($"{baseUrl}/odata/Batch?$filter=BatchId eq {batchId}");

            if (batchResponse.IsSuccessStatusCode)
            {
                var jsonString = await batchResponse.Content.ReadAsStringAsync();

                // Parse the JSON response to extract the 'value' field
                var jsonDocument = JsonDocument.Parse(jsonString);
                var batchData = jsonDocument.RootElement.GetProperty("value");

                // Since 'value' is an array, even if there's one object, deserialize to a list and take the first element
                var batchList = JsonSerializer.Deserialize<List<ListBatchDTO>>(batchData.ToString(), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Assign the first batch object (if exists)
                Batch = batchList?.FirstOrDefault();
            }


            // Gọi API OData để lấy danh sách hoa theo BatchId
            var response = await _httpClient.GetAsync($"{baseUrl}/odata/Flower?$filter=BatchId eq {batchId}");

            if (response.IsSuccessStatusCode)
            {
                var flowerJsonString = await response.Content.ReadAsStringAsync();
                var flowerData = JsonDocument.Parse(flowerJsonString).RootElement.GetProperty("value");

                Flowers = JsonSerializer.Deserialize<List<ListFlowerDTO>>(flowerData.ToString(), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }
    }

}
