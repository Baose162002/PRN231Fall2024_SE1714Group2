using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN231_2_EventFlowerExchange_FE.Service;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static PRN231_2_EventFlowerExchange_FE.Pages.BatchPages.BatchIndexModel;
using System.Text.Json.Serialization;
using static BusinessObject.Enum.EnumList;
using System.IdentityModel.Tokens.Jwt;

namespace PRN231_2_EventFlowerExchange_FE.Pages.FlowerPages
{
    public class CreateFlowerModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly CloudinaryService _cloudinaryService;

        public CreateFlowerModel(HttpClient httpClient, IConfiguration configuration, CloudinaryService cloudinaryService)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _cloudinaryService = cloudinaryService;
        }

        [BindProperty]
        public CreateFlowerDTO FlowerInput { get; set; }
        public List<ListBatchDTO> Batches { get; set; }

        public async Task<IActionResult> OnGetAsync(int companyId)
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

            string odataQuery = ""; // Mặc định cho Admin

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
            var apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}{odataQuery}";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var odataResponse = JsonSerializer.Deserialize<ODataResponse<ListBatchDTO>>(jsonString, options);
                Batches = odataResponse.Value; // Gán danh sách batch vào thuộc tính Batches
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error fetching batch information: {errorMessage}");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                // Upload the image to Cloudinary and get the URL
                FlowerInput.Image = await _cloudinaryService.UploadImageAsync(imageFile);
            }


            var json = JsonSerializer.Serialize(FlowerInput);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/api/flower/create";
            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                // Redirect on success
                return RedirectToPage("/Index");
            }
            else
            {
                // Handle error response
                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error: {errorMessage}");

                // Return to the same page with error message
                return Page();
            }
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
