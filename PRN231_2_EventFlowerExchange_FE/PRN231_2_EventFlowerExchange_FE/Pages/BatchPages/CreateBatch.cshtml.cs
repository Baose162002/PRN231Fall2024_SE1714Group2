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
    }

   
}
