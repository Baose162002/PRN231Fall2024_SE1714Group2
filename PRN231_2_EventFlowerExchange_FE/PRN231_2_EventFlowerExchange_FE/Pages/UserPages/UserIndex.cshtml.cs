using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PRN231_2_EventFlowerExchange_FE.Pages.UserPages
{
    public class UserIndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public UserIndexModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        public List<ListUserDTO> Users { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            /* var role = HttpContext.Session.GetString("UserRole");

             if (string.IsNullOrEmpty(token) || role != "Admin")
             {
                 return RedirectToPage("/Login");
             }
 */

            string odataQuery = "/odata/user"; // M?c ??nh cho Admin

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"{_baseApiUrl}{odataQuery}");


            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var odataResponse = JsonSerializer.Deserialize<ODataResponse<ListUserDTO>>(jsonString, options);
                Users = odataResponse.Value;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to load users.");
            }

            return Page();
        }

        public class ODataResponse<T>
        {
            [JsonPropertyName("@odata.context")]
            public string OdataContext { get; set; }
            public List<T> Value { get; set; }
        }
    }
}