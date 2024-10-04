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

        public void OnGet()
        {
            Input = new CreateBatchDTO();
        }

        public async Task<IActionResult> OnPostAsync()
        {
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
                // First, create the flower
                var flowerInput = new CreateFlowerDTO
                {
                    Name = Request.Form["FlowerName"],
                    Type = Input.FlowerType,
                    Image = Request.Form["FlowerImage"],
                    Description = Input.Description,
                    PricePerUnit = Input.PricePerUnit,
                    Origin = Request.Form["Origin"],
                    Color = Request.Form["Color"]
                };

                var flowerJson = JsonSerializer.Serialize(flowerInput);
                var flowerContent = new StringContent(flowerJson, Encoding.UTF8, "application/json");

                var flowerApiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/flower"; // Update with your flower API URL
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var flowerResponse = await _httpClient.PostAsync(flowerApiUrl, flowerContent);

                if (flowerResponse.IsSuccessStatusCode)
                {
                    var responseContent = await flowerResponse.Content.ReadAsStringAsync();
                    var createdFlower = JsonSerializer.Deserialize<ListFlowerDTO>(responseContent); // Define this class to match the response
                    Input.FlowerId = createdFlower.FlowerId; // Set the FlowerId in the BatchInput
                }
                else
                {
                    var errorContent = await flowerResponse.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Error creating flower. Status code: {errorContent}");
                    return Page();
                }



                // Convert EntryDate to the required format "dd/MM/yyyy"
                if (DateTime.TryParseExact(Input.EntryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    Input.EntryDate = parsedDate.ToString("dd/MM/yyyy");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid date format.");
                    return Page();
                }

                // Get UserId from claims
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // Fetch CompanyId using UserId
                var company = await GetCompanyByUserIdAsync(userId);
                if (company != null)
                {
                    Input.CompanyId = company.CompanyId; // Assign CompanyId to Input
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Company not found for the user.");
                    return Page();
                }

                var json = JsonSerializer.Serialize(Input);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/batch";

                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Batch created successfully!";
                    return RedirectToPage("/BatchIndex");
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
            var apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/api/Company/{userId}";

            // Gọi API để lấy thông tin công ty
            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var company = JsonSerializer.Deserialize<CompanyDTO>(content);
                return company;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error fetching company information. Status code: {response.StatusCode}, Content: {errorContent}");
                throw new Exception($"Error fetching company information. Status code: {response.StatusCode}");
            }
        }

    }


}
