using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Globalization; // Added for date conversion
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using BusinessObject.DTO.Request;
using System.Security.Claims;
using BusinessObject.DTO.Response;
using System.Net.Http.Headers;
using BusinessObject;
using PRN231_2_EventFlowerExchange_FE.Service;

namespace PRN231_2_EventFlowerExchange_FE.Pages.BatchPages
{
    public class CreateBatchModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CreateBatchModel> _logger;

        public CreateBatchModel(HttpClient httpClient, IConfiguration configuration, ILogger<CreateBatchModel> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        [BindProperty]
        public CreateBatchDTO Input { get; set; }

       
        public List<CompanyDTO> Companies { get; set; } = new List<CompanyDTO>();

        public async Task<IActionResult> OnGetAsync()
        {
            Input = new CreateBatchDTO();

            var token = HttpContext.Session.GetString("JWTToken");
            var role = HttpContext.Session.GetString("UserRole");


            if (role == "Admin") // Assuming you store the role as a string
            {
                Companies = await GetCompaniesAsync(token); // Fetch companies
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Input.ImgFlower");

            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var token = HttpContext.Session.GetString("JWTToken");
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("JWT Token is missing from session");
                    ModelState.AddModelError(string.Empty, "You are not authenticated. Please log in.");
                    return Page();
                }

                var userRole = HttpContext.Session.GetString("UserRole");

                if (userRole == "Seller")
                {
                    // Logic for fetching CompanyId for Company role remains unchanged
                    var userIdString = HttpContext.Session.GetString("UserId");
                    if (string.IsNullOrEmpty(userIdString))
                    {
                        ModelState.AddModelError(string.Empty, "User ID not found in session.");
                        return Page();
                    }

                    if (!int.TryParse(userIdString, out int userId))
                    {
                        ModelState.AddModelError(string.Empty, "Invalid User ID.");
                        return Page();
                    }

                    var company = await GetCompanyByUserIdAsync(userIdString);
                    if (company == null)
                    {
                        ModelState.AddModelError(string.Empty, "Company not found for the user.");
                        return Page();
                    }
                    Input.CompanyId = company.CompanyId; // Set the Company ID for Company role
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid user role.");
                    return Page();
                }


                var json = JsonSerializer.Serialize(Input);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/api/batch";

                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Batch created successfully!";
                    return RedirectToPage("/BatchPages/BatchIndex");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Error creating batch. Status code: {errorContent}");
                    return Page();
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while creating batch");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                return Page();
            }
        }

        private async Task<List<CompanyDTO>> GetCompaniesAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/api/company"; // Adjust API URL accordingly

            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<CompanyDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return new List<CompanyDTO>(); // Return empty list on failure
        }

        private async Task<CompanyDTO> GetCompanyByUserIdAsync(string userId)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("JWT Token is missing from session");
            }

            // Thiết lập Header Authorization với JWT Token
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Tạo URL cho API để lấy thông tin công ty
            var apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/api/Company/user/{userId}";

            // Gọi API để lấy thông tin công ty
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
                _logger.LogError($"Error fetching company information. Status code: {response.StatusCode}, Content: {errorContent}");
                return null;



            }
        }

    }


}
