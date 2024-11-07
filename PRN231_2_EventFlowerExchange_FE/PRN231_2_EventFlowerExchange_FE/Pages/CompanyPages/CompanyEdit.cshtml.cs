using BusinessObject.Dto.Response;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PRN231_2_EventFlowerExchange_FE.Pages.CompanyPages
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public EditModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        [BindProperty]
        public UpdateCompanyDTO UpdateCompanyDTO { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var token = HttpContext.Session.GetString("JWTToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"{_baseApiUrl}/api/company/{id}");

            if (response.IsSuccessStatusCode)
            {
                var company = await response.Content.ReadFromJsonAsync<CompanyDTO>();
                UpdateCompanyDTO = new UpdateCompanyDTO
                {
                    CompanyName = company.CompanyName,
                    CompanyAddress = company.CompanyAddress,
                    CompanyDescription = company.CompanyDescription,
                    TaxNumber = company.TaxNumber,
                    PostalCode = company.PostalCode,
                    City = company.City,
                    UserId = company.UserId,
                    Status = Enum.TryParse<BusinessObject.Enum.EnumList.Status>(company.Status, out var status)
                        ? status
                        : BusinessObject.Enum.EnumList.Status.Active // Use a default or fallback value here
                };
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ModelState.AddModelError(string.Empty, "Token is invalid or has expired.");
                return RedirectToPage("/Login/Login");
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"Error loading company: {response.ReasonPhrase}");
                return RedirectToPage("/CompanyPages/Index");
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors in the form.";
                return Page();
            }

            var token = HttpContext.Session.GetString("JWTToken");

            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Session has expired. Please log in again.";
                return RedirectToPage("/Login/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_baseApiUrl}/api/company/{id}", UpdateCompanyDTO);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Company information updated successfully!";
                    return RedirectToPage("/CompanyPages/CompanyEdit", new { id });
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    TempData["ErrorMessage"] = "Token is invalid or has expired.";
                    return RedirectToPage("/Login/Login");
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseContent) &&
                        (responseContent.Trim().StartsWith("{") || responseContent.Trim().StartsWith("[")))
                    {
                        var jsonDocument = JsonDocument.Parse(responseContent);
                        if (jsonDocument.RootElement.TryGetProperty("message", out var messageElement))
                        {
                            TempData["ErrorMessage"] = messageElement.GetString();
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "An error occurred while updating the company.";
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"Error: {responseContent}";
                    }

                    return Page();
                }
            }
            catch (HttpRequestException e)
            {
                TempData["ErrorMessage"] = $"Error connecting to the server: {e.Message}";
                return Page();
            }
        }
    }
}