using BusinessObject;
using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace PRN231_2_EventFlowerExchange_FE.Pages.BatchPages
{
    public class BatchIndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;
        private readonly IConfiguration _configuration;  // Đã thêm _configuration vào

        public BatchIndexModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
            _configuration = configuration;
        }

        public List<ListBatchDTO> Batches { get; set; }
        public List<CompanyDTO> Companies { get; set; } // Thêm danh sách công ty

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("JWTToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login/Login");
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");

            string odataQuery = "/odata/batch"; // Mặc định cho Admin

            if (userRole == "Seller" && userId != null)
            {
                var company = await GetCompanyByUserIdAsync(userId);
                if (company != null)
                {
                    odataQuery = $"/odata/batch?$filter=CompanyId eq {company.CompanyId}";
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Unable to retrieve company information.");
                    return Page();
                }
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Gọi API lấy danh sách công ty
            var companyResponse = await _httpClient.GetAsync($"{_baseApiUrl}/api/company");
            if (companyResponse.IsSuccessStatusCode)
            {
                var companyOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                Companies = await companyResponse.Content.ReadFromJsonAsync<List<CompanyDTO>>(companyOptions);
            }
            else
            {
                var errorContent = await companyResponse.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error fetching company list: {errorContent}");
                return Page();
            }

            // Gọi API lấy danh sách batch

            var response = await _httpClient.GetAsync($"{_baseApiUrl}{odataQuery}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var odataResponse = JsonSerializer.Deserialize<ODataResponse<ListBatchDTO>>(jsonString, options);
                Batches = odataResponse.Value;
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ModelState.AddModelError(string.Empty, "Invalid or expired token.");
                return RedirectToPage("/Login");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error fetching batch list: {errorContent}");
            }

            return Page();
        }


        private async Task<CompanyDTO> GetCompanyByUserIdAsync(string userId)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("JWT Token is missing from session");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/api/Company/user/{userId}";

            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var company = JsonSerializer.Deserialize<CompanyDTO>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return company;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error fetching company information: {errorContent}");
                return null;
            }
        }
        public class ODataResponse<T>
        {
            [JsonPropertyName("@odata.context")]
            public string OdataContext { get; set; }
            public List<T> Value { get; set; }
        }
    }
}
