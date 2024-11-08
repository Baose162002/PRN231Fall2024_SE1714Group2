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
        private readonly CloudinaryService _cloudinaryService;

        public CreateBatchModel(HttpClient httpClient, IConfiguration configuration, ILogger<CreateBatchModel> logger, CloudinaryService cloudinaryService)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _cloudinaryService = cloudinaryService;
        }

        [BindProperty]
        public CreateBatchDTO Input { get; set; }

        public List<CompanyDTO> Companies { get; set; } = new List<CompanyDTO>();

        // Add a list for flowers to be created
        [BindProperty]
        public List<CreateFlowerDTOs> Flowers { get; set; } = new List<CreateFlowerDTOs>();

        public async Task<IActionResult> OnGetAsync()
        {
            Input = new CreateBatchDTO();
            Flowers = new List<CreateFlowerDTOs>();

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
            try
            {

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

                // Prepare flowersWithImages (convert CreateFlowerDTO to FlowerDTOs)
                var flowersWithImages = new List<FlowerDTOs>();

                foreach (var flower in Flowers) // Assuming `Flowers` is the list of flowers
                {
                    if (flower.Image != null)
                    {
                        // Upload the image to Cloudinary and get the URL
                        var imageUrl = await _cloudinaryService.UploadImageAsync1(flower.Image);

                        // Assuming FlowerDTOs contains the necessary flower details
                        var flowerDTO = new FlowerDTOs
                        {
                            Name = flower.Name,
                            Type = flower.Type,
                            Origin = flower.Origin,
                            Color = flower.Color,
                            PricePerUnit = flower.Price,
                            RemainingQuantity = flower.Quantity,
                            Description = flower.Description,
                            Image = imageUrl // URL returned by Cloudinary after image upload
                        };

                        flowersWithImages.Add(flowerDTO);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Image is required for each flower.");
                        return Page();
                    }
                }

                // Assuming CreateBatchAndFlowerDTO expects List<FlowerDTOs>
                var batch = new CreateBatchAndFlowerDTO
                {
                    BatchName = Input.BatchName,
                    EventName = Input.EventName,
                    EventDate = Input.EventDate,
                    EntryDate = Input.EntryDate,
                    Description = Input.Description,
                    CompanyId = Input.CompanyId,
                    Flowers = flowersWithImages
                };

                // Call an API to create the batch and flowers
                var apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/api/batch/Create";
                var response = await _httpClient.PostAsJsonAsync(apiUrl, batch);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/BatchPages/BatchIndex");  // Redirect to the batch list page
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
                _logger.LogError($"Error occurred while creating batch: {ex.Message}");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the batch. Please try again later.");
                return Page();
            }
        }

        private async Task<List<CompanyDTO>> GetCompaniesAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/api/company";

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

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

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
                _logger.LogError($"Error fetching company information. Status code: {response.StatusCode}, Content: {errorContent}");
                return null;
            }
        }
    }



}
