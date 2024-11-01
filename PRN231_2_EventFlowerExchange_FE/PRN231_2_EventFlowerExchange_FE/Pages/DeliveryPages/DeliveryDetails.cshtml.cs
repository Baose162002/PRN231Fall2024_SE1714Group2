using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObject;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Text.Json;
using BusinessObject.DTO.Response;
using static BusinessObject.Enum.EnumList;

namespace PRN231_2_EventFlowerExchange_FE.Pages.DeliveryPages
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public DetailsModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public List<ListDeliveryDTO> Deliveries { get; set; } = new List<ListDeliveryDTO>();

        public async Task<IActionResult> OnGetAsync()
        {
            // Retrieve JWT token and UserId from session
            var token = HttpContext.Session.GetString("JWTToken");
            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            // Check for valid token and user role
            if (string.IsNullOrEmpty(token) || userRole != "DeliveryPersonnel")
            {
                ModelState.AddModelError(string.Empty, "Unauthorized access. Only Delivery Personnel can view this page.");
                return RedirectToPage("/Login/Login");
            }

            // Set Authorization header with Bearer token
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Construct the API URL with the UserId for filtering deliveries
            string apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/api/Delivery/{userId}";
            var response = await _httpClient.GetAsync(apiUrl);

            // Check if the API response is successful
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    Converters = { new DeliveryStatusConverter() } // Add the custom converter
                };

                // Deserialize the response to get the list of deliveries
                Deliveries = await response.Content.ReadFromJsonAsync<List<ListDeliveryDTO>>(options);
            }
            else
            {
                // Display error message if API call fails
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error fetching delivery data: {errorContent}");
                return Page();
            }

            return Page();
        }
        public class DeliveryStatusConverter : JsonConverter<DeliveryStatus>
        {
            public override DeliveryStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                // Attempt to parse the enum from a string
                if (reader.TokenType == JsonTokenType.String && Enum.TryParse(reader.GetString(), true, out DeliveryStatus status))
                {
                    return status;
                }

                throw new JsonException($"Unable to convert {reader.GetString()} to DeliveryStatus.");
            }

            public override void Write(Utf8JsonWriter writer, DeliveryStatus value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }
    }
}
