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

            var response = await _httpClient.GetAsync($"{_baseApiUrl}/user/{id}");

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
                return RedirectToPage("/UserPages/UserIndex/UserPages/UserIndex");
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

            var response = await _httpClient.PutAsJsonAsync($"{_baseApiUrl}/user/{id}", UpdateUserDTO);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/UserPages/UserIndex");
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ModelState.AddModelError(string.Empty, "Token is invalid or has expired.");
                return RedirectToPage("/Login/Login");
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"Error updating user: {response.ReasonPhrase}");
                return Page();
            }
        }
    }
}