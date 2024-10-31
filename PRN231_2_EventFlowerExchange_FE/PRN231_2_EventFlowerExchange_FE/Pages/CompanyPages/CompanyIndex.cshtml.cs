using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessObject.DTO.Response;
using System.Text.Json;
using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace PRN231_2_EventFlowerExchange_FE.Pages.CompanyPages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public IndexModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
            Companies = new List<CompanyDTO>();
        }

        // Properties for company list and pagination
        public List<CompanyDTO> Companies { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public string SearchTerm { get; set; }
        public string SearchColumn { get; set; }

        public async Task<IActionResult> OnGetAsync(int? pageNumber, int? pageSize, string searchTerm, string searchColumn)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Login/Login");

            CurrentPage = pageNumber ?? CurrentPage;
            PageSize = pageSize ?? PageSize;
            SearchTerm = searchTerm?.Trim();
            SearchColumn = searchColumn?.Trim();

            var odataQueryBuilder = new List<string>
    {
        "$count=true",
        $"$skip={(CurrentPage - 1) * PageSize}",
        $"$top={PageSize}"
    };

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                var filterConditions = new List<string>();
                var searchTermLower = SearchTerm.ToLower();

                if (string.IsNullOrEmpty(SearchColumn) || SearchColumn.ToLower() == "all")
                {
                    filterConditions.AddRange(new[]
                    {
                $"contains(tolower(CompanyName), '{searchTermLower}')",
                $"contains(tolower(CompanyAddress), '{searchTermLower}')",
                $"contains(tolower(CompanyDescription), '{searchTermLower}')"
            });
                }
                else
                {
                    switch (SearchColumn)
                    {
                        case "CompanyName":
                            filterConditions.Add($"contains(tolower(CompanyName), '{searchTermLower}')");
                            break;
                        case "CompanyAddress":
                            filterConditions.Add($"contains(tolower(CompanyAddress), '{searchTermLower}')");
                            break;
                        case "CompanyDescription":
                            filterConditions.Add($"contains(tolower(CompanyDescription), '{searchTermLower}')");
                            break;
                    }
                }

                if (filterConditions.Any())
                {
                    odataQueryBuilder.Add($"$filter={string.Join(" or ", filterConditions)}");
                }
            }

            string odataQuery = $"/odata/company?{string.Join("&", odataQueryBuilder)}";

            try
            {
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

                    var odataResponse = JsonSerializer.Deserialize<ODataResponse<CompanyDTO>>(jsonString, options);
                    Companies = odataResponse?.Value ?? new List<CompanyDTO>();
                    TotalCount = odataResponse?.Count ?? 0;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to load companies.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            }

            return Page();
        }

        // OData response model
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
