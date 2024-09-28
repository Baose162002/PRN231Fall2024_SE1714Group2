using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PRN231_2_EventFlowerExchange_FE.Pages.BatchPages
{
    public class BatchIndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public BatchIndexModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        public List<Batch> Batches { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/batch");

            if (response.IsSuccessStatusCode)
            {
                Batches = await response.Content.ReadFromJsonAsync<List<Batch>>();
            }
            else
            {

                ModelState.AddModelError(string.Empty, "Không thể tải danh sách batch.");
            }

            return Page();
        }
    }
}
