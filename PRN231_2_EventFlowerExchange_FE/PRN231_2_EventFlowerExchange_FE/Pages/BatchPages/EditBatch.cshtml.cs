using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using BusinessObject.DTO.Request;
using System.Globalization;

namespace PRN231_2_EventFlowerExchange_FE.Pages.BatchPages
{
    public class EditBatchModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EditBatchModel> _logger;

        public EditBatchModel(HttpClient httpClient, IConfiguration configuration, ILogger<EditBatchModel> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        [BindProperty]
        public UpdateBatchDTO Input { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("JWT Token is missing from session");
                return RedirectToPage("/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/batch/{id}";
            _logger.LogInformation($"Sending GET request to {apiUrl}");

            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                Input = JsonSerializer.Deserialize<UpdateBatchDTO>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                _logger.LogInformation("Batch data loaded successfully");
                return Page();
            }
            else
            {
                _logger.LogError($"Error fetching batch data. Status code: {response.StatusCode}");
                ModelState.AddModelError(string.Empty, "Failed to load batch data.");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
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
                if (DateTime.TryParseExact(Input.EntryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    Input.EntryDate = parsedDate.ToString("dd/MM/yyyy");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid date format.");
                    return Page();
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var json = JsonSerializer.Serialize(Input);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/batch/{id}";

                _logger.LogInformation($"Sending PUT request to {apiUrl}");
                var response = await _httpClient.PutAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Batch updated successfully");
                    TempData["SuccessMessage"] = "Batch updated successfully!";
                    return RedirectToPage("./BatchIndex");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error updating batch. Status code: {response.StatusCode}, Content: {errorContent}");
                    ModelState.AddModelError(string.Empty, $"Error updating batch. Status code: {errorContent}");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while updating batch");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                return Page();
            }
        }
    }


}
