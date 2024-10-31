using BusinessObject.Dto.Response;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PRN231_2_EventFlowerExchange_FE.Pages.UserPages
{
    public class UserEditModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public UserEditModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        [BindProperty]
        public UpdateUserDTO UpdateUserDTO { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var token = HttpContext.Session.GetString("JWTToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"{_baseApiUrl}/api/user/{id}");

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<UserResponseDto>();
                UpdateUserDTO = new UpdateUserDTO
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Address = user.Address,
                    Role = Enum.Parse<BusinessObject.Enum.EnumList.UserRole>(user.Role)
                };
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ModelState.AddModelError(string.Empty, "Token is invalid or has expired.");
                return RedirectToPage("/Login/Login");
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"Error loading user: {response.ReasonPhrase}");
                return RedirectToPage("/UserPages/UserIndex");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var token = HttpContext.Session.GetString("JWTToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_baseApiUrl}/api/user/{id}", UpdateUserDTO);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "User information updated successfully!";
                    return RedirectToPage("/UserPages/UserEdit", new { id });
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ModelState.AddModelError(string.Empty, "Token is invalid or has expired.");
                    return RedirectToPage("/Login/Login");
                }
                else
                {
                    // Check if response content is JSON before parsing
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Attempt to parse JSON only if it starts with '{' or '['
                    if (!string.IsNullOrEmpty(responseContent) &&
                        (responseContent.Trim().StartsWith("{") || responseContent.Trim().StartsWith("[")))
                    {
                        var jsonDocument = JsonDocument.Parse(responseContent);
                        if (jsonDocument.RootElement.TryGetProperty("message", out var messageElement))
                        {
                            ModelState.AddModelError(string.Empty, messageElement.GetString());
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "An error occurred while updating the user.");
                        }
                    }
                    else
                    {
                        // Fallback if response is not JSON
                        ModelState.AddModelError(string.Empty, $"Error: {responseContent}");
                    }

                    return Page();
                }
            }
            catch (HttpRequestException e)
            {
                ModelState.AddModelError(string.Empty, $"Error connecting to the server: {e.Message}");
                return Page();
            }
        }
    }
}
