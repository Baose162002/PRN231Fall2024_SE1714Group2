using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;

namespace PRN231_2_EventFlowerExchange_FE.Pages.FlowerPages
{
    public class FlowerIndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;
        public List<ListFlowerDTO> Flowers { get; set; }

        public FlowerIndexModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task OnGetAsync()
        {
            // Lấy token từ session (nếu cần)
            var token = HttpContext.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }


            var odataUrl = $"{_baseApiUrl}/odata/Flower?$filter=Status eq 'Active' and (Condition eq 'Fresh')";
            var response = await _httpClient.GetAsync(odataUrl);
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var odataResponse = await response.Content.ReadFromJsonAsync<ODataResponse<ListFlowerDTO>>(options);

                Flowers = odataResponse?.Value ?? new List<ListFlowerDTO>();
            }

           
            else
            {
                Flowers = new List<ListFlowerDTO>();
                ModelState.AddModelError(string.Empty, "Not found flower");
            }
        }
        public class ODataResponse<T>
        {
            public List<T> Value { get; set; }
        }
    }
}
