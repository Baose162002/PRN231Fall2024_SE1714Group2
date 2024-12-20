﻿using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR.Protocol;
using System.Net.Http.Headers;
using System.Text.Json;
using static PRN231_2_EventFlowerExchange_FE.Pages.BatchPages.BatchIndexModel;

namespace PRN231_2_EventFlowerExchange_FE.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public IndexModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        public List<ListFlowerDTO> Flowers { get; set; }
        public string ApiMessage { get; set; }

        public string UserRole { get; set; }

        public async Task OnGetAsync()
        {
            UserRole = HttpContext.Session.GetString("UserRole"); // Default to "Customer" if role is not set

            // Lấy token từ session (nếu cần)
            var token = HttpContext.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var reviewResponse = await _httpClient.PostAsync($"{_baseApiUrl}/api/Batch/CheckAndUpdateBatchStatus", null);
            if (reviewResponse.IsSuccessStatusCode)
            {
                var jsonResponse = await reviewResponse.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(jsonResponse);

                if (apiResponse != null)
                {
                    ApiMessage = apiResponse.Message;
                }
            }
            else
            {
                var errorContent = await reviewResponse.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error updating batch status: {errorContent}");
            }
            // Gọi API để lấy danh sách hoa
            var odataUrl = $"{_baseApiUrl}/odata/Flower?$filter=Status eq 'Active' and Condition eq 'Fresh' and FlowerStatus eq 'Available'";
            var response = await _httpClient.GetAsync(odataUrl);
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var odataResponse = await response.Content.ReadFromJsonAsync<ODataResponse<ListFlowerDTO>>(options);

                Flowers = odataResponse?.Value ?? new List<ListFlowerDTO>();
            }
            else
            {
                Flowers = new List<ListFlowerDTO>();
                ModelState.AddModelError(string.Empty, "Not found flower");
            }
           
        }

        public class ODataResponse<T>
        {
            public List<T> Value { get; set; }
        }

        public class ApiResponse
        {
            public string Message { get; set; }
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAddToCartAsync([FromBody] AddToCartRequest request)
        {
            var flower = await GetFlowerById(request.FlowerId);
            if (flower != null)
            {
                var cartItem = new CartItemDTO
                {
                    FlowerId = flower.FlowerId.ToString(),
                    Name = flower.Name,
                    Description = flower.Description,
                    PricePerUnit = flower.PricePerUnit,
                    Image = flower.Image,
                    Quantity = 1
                };
                var cartCount = AddToCart(cartItem);
                return new JsonResult(new { success = true, cartCount = cartCount });
            }
            return new JsonResult(new { success = false, message = "Flower not found" });
        }

        private async Task<ListFlowerDTO> GetFlowerById(string flowerId)
        {
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/api/Flower/GetBy/{flowerId}");
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return await response.Content.ReadFromJsonAsync<ListFlowerDTO>(options);
            }
            return null;
        }

        private int AddToCart(CartItemDTO flower)
        {
            var cartJson = HttpContext.Request.Cookies["cartItems"];
            List<CartItemDTO> cartItems = string.IsNullOrEmpty(cartJson)
                ? new List<CartItemDTO>()
                : JsonSerializer.Deserialize<List<CartItemDTO>>(cartJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var existingItem = cartItems.FirstOrDefault(x => x.FlowerId == flower.FlowerId);
            if (existingItem != null)
            {
                existingItem.Quantity += 1; // Tăng số lượng của sản phẩm đã tồn tại
            }
            else
            {
                flower.Quantity = 1; // Nếu chưa có thì khởi tạo số lượng là 1
                cartItems.Add(flower);
            }

            var options = new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) };
            HttpContext.Response.Cookies.Append("cartItems", JsonSerializer.Serialize(cartItems), options);

            return cartItems.Count; // Trả về số lượng sản phẩm khác nhau trong giỏ hàng
        }



        public JsonResult OnGetGetCartCount()
        {
            var cartJson = HttpContext.Request.Cookies["cartItems"];
            List<CartItemDTO> cartItems = string.IsNullOrEmpty(cartJson)
                ? new List<CartItemDTO>()
                : JsonSerializer.Deserialize<List<CartItemDTO>>(cartJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            int cartCount = cartItems.Sum(item => item.Quantity); // Tính tổng số lượng sản phẩm
            return new JsonResult(new { count = cartCount });
        }


    }

    public class AddToCartRequest
    {
        public string FlowerId { get; set; }
    }


}

