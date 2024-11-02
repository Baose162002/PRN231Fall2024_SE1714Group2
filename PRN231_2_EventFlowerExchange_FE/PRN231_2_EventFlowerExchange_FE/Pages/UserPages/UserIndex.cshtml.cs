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
        public int PageSize { get; set; } = 5;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public string SearchTerm { get; set; }
        public string SearchColumn { get; set; }



        public async Task<IActionResult> OnGetAsync(int? pageNumber, int? pageSize, string searchTerm, string searchColumn)
        {

            var token = HttpContext.Session.GetString("JWTToken");
            var userRole = HttpContext.Session.GetString("UserRole");

            // Check if the user is authenticated and has the "Admin" role
            if (string.IsNullOrEmpty(token) || userRole != "Admin")
            {
                ModelState.AddModelError(string.Empty, "Unauthorized access. Only Admins can view this page.");
                return RedirectToPage("/Index");
            }

            if (pageNumber.HasValue && pageNumber > 0)
            {
                CurrentPage = pageNumber.Value;
            }

            if (pageSize.HasValue && pageSize > 0)
            {
                PageSize = pageSize.Value;
            }

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
                    // Xử lý tìm kiếm tất cả các cột
                    filterConditions.AddRange(new[]
                    {
                        $"contains(tolower(FullName), '{searchTermLower}')",
                        $"contains(tolower(Email), '{searchTermLower}')",
                        $"contains(tolower(Phone), '{searchTermLower}')",
                        $"contains(tolower(Address), '{searchTermLower}')",
                        $"contains(tolower(Role), '{searchTermLower}')",
                        $"contains(tolower(Status), '{searchTermLower}')"
                    });
                }
                else
                {
                    // Xử lý tìm kiếm theo cột cụ thể
                    switch (SearchColumn.ToLower())
                    {
                        case "fullname":
                            filterConditions.Add($"contains(tolower(FullName), '{searchTermLower}')");
                            break;
                        case "email":
                            filterConditions.Add($"contains(tolower(Email), '{searchTermLower}')");
                            break;
                        case "phone":
                            filterConditions.Add($"contains(tolower(Phone), '{searchTermLower}')");
                            break;
                        case "address":
                            filterConditions.Add($"contains(tolower(Address), '{searchTermLower}')");
                            break;
                        case "role":
                            filterConditions.Add($"contains(tolower(Role), '{searchTermLower}')");
                            break;
                        case "status":
                            filterConditions.Add($"contains(tolower(Status), '{searchTermLower}')");
                            break;
                        default:
                            filterConditions.Add($"contains(tolower(FullName), '{searchTermLower}')");
                            break;
                    }
                }

                if (filterConditions.Any())
                {
                    odataQueryBuilder.Add($"$filter={string.Join(" or ", filterConditions)}");
                }
            }

            string odataQuery = $"/odata/user?{string.Join("&", odataQueryBuilder)}";

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

                    var odataResponse = JsonSerializer.Deserialize<ODataResponse<ListUserDTO>>(jsonString, options);
                    Users = odataResponse.Value;
                    TotalCount = odataResponse.Count;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to load users.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return Page();
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