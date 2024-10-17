using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PRN231_2_EventFlowerExchange_FE.Pages.UserPages
{
    public class UserIndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public UserIndexModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        public List<ListUserDTO> Users { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public async Task<IActionResult> OnGetAsync(int? pageNumber, int? pageSize)
        {
            var token = HttpContext.Session.GetString("JWTToken");

            if (pageNumber.HasValue && pageNumber > 0)
            {
                CurrentPage = pageNumber.Value;
            }

            if (pageSize.HasValue && pageSize > 0)
            {
                PageSize = pageSize.Value;
            }

            string odataQuery = $"/odata/user?$count=true&$skip={(CurrentPage - 1) * PageSize}&$top={PageSize}";

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{_baseApiUrl}{odataQuery}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var odataResponse = JsonSerializer.Deserialize<ODataResponse<ListUserDTO>>(jsonString, options);
                Users = odataResponse.Value;
                TotalCount = odataResponse.Count;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to load users.");
            }

            return Page();
        }

        public class ODataResponse<T>
        {
            [JsonPropertyName("@odata.context")]
            public string OdataContext { get; set; }

            [JsonPropertyName("@odata.count")]
            public int Count { get; set; }

            public List<T> Value { get; set; }
        }
    }
}