using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using System.Net.Http.Headers;
using PRN231_2_EventFlowerExchange_FE.Service;

namespace PRN231_2_EventFlowerExchange_FE.Pages.OrderPages
{
    public class CreateOrderModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CreateOrderModel> _logger;

        public CreateOrderModel(HttpClient httpClient, IConfiguration configuration, ILogger<CreateOrderModel> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        [BindProperty]
        public CreateOrderDTO Input { get; set; }

        public List<ListBatchDTO> AvailableBatches { get; set; } = new List<ListBatchDTO>();
        public List<ListFlowerDTO> Flowers { get; set; } = new List<ListFlowerDTO>();

        public async Task<IActionResult> OnGetAsync()
        {
            Input = new CreateOrderDTO();
            Flowers = await GetFlowersAsync(); // Fetch list of available flowers

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
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

            // Step 1: Validate selected batches against flower ID
            var availableBatches = await GetBatchesByFlowerId(Input.FlowerId);
            var validBatchIds = availableBatches.Select(b => b.BatchId).ToList();

            foreach (var selectedBatch in Input.SelectedBatches)
            {
                if (!validBatchIds.Contains(selectedBatch.BatchId))
                {
                    ModelState.AddModelError(string.Empty, $"BatchId {selectedBatch.BatchId} is not valid for FlowerId {Input.FlowerId}");
                    return Page();
                }

                // Step 2: Check if selected quantity is available in the batch
                var batch = availableBatches.First(b => b.BatchId == selectedBatch.BatchId);
                if (batch.BatchQuantity < selectedBatch.QuantityOrdered)
                {
                    ModelState.AddModelError(string.Empty, $"BatchId {selectedBatch.BatchId} does not have enough quantity. Available: {batch.BatchQuantity}");
                    return Page();
                }
            }

            // Step 3: Send the order creation request to the API
            var json = JsonSerializer.Serialize(Input);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/order";

            var response = await _httpClient.PostAsync(apiUrl, content);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Order created successfully!";
                return RedirectToPage("/OrderPages/OrderIndex");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error creating order. Status code: {errorContent}");
                return Page();
            }
        }

        // Fetch available flowers from API
        private async Task<List<ListFlowerDTO>> GetFlowersAsync()
        {
            var apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/flower";
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<ListFlowerDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return new List<ListFlowerDTO>();
        }

        // Fetch available batches based on selected flower
        private async Task<List<ListBatchDTO>> GetBatchesByFlowerId(int flowerId)
        {
            var apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/Batch/getBatchesByFlower/{flowerId}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<ListBatchDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return new List<ListBatchDTO>();
        }
    }
}
