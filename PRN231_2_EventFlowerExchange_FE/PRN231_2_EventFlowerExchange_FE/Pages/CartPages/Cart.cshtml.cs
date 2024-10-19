using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Common;
using PRN231_2_EventFlowerExchange_FE.Service;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PRN231_2_EventFlowerExchange_FE.Pages.CartPages
{
    public class CartModel : PageModel
    {
        public List<CartItemDTO> CartItems { get; set; } = new List<CartItemDTO>();
        private readonly PaymentService _paymentService;
        
        public CartModel(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }
       

        private List<CartItemDTO> GetCartItems()
        {
            var cartJson = HttpContext.Request.Cookies["cartItems"];

            if (string.IsNullOrEmpty(cartJson))
            {
                // No cart items in the cookie, return an empty list
                return new List<CartItemDTO>();
            }

            try
            {
                // Deserialize using case-insensitive property matching
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<List<CartItemDTO>>(cartJson, options) ?? new List<CartItemDTO>();
            }
            catch (JsonException ex)
            {
                // Log the error (optional) and return an empty list if deserialization fails
                Console.WriteLine($"Error deserializing cart items: {ex.Message}");
                return new List<CartItemDTO>();
            }
        }


        public void OnGet()
        {
            CartItems = GetCartItems();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult OnPostUpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            var cartJson = HttpContext.Request.Cookies["cartItems"];
            var cartItems = string.IsNullOrEmpty(cartJson) ? new List<CartItemDTO>() : JsonSerializer.Deserialize<List<CartItemDTO>>(cartJson);

            var item = cartItems.FirstOrDefault(x => x.FlowerId == request.FlowerId);
            if (item != null)
            {
                item.Quantity = request.Quantity;
            }

            var updatedCartJson = JsonSerializer.Serialize(cartItems);
            HttpContext.Response.Cookies.Append("cartItems", updatedCartJson, new CookieOptions { Path = "/" });

            return new JsonResult(cartItems); 
        }

        [ValidateAntiForgeryToken]
        public JsonResult OnPostDeleteItem([FromBody] DeleteItemRequest request)
        {
            try
            {
                // Get current cart items
                var cartItems = GetCartItems();

                // Find and remove the item
                var itemToRemove = cartItems.FirstOrDefault(x => x.FlowerId == request.FlowerId);
                if (itemToRemove != null)
                {
                    cartItems.Remove(itemToRemove);

                    // Update cookie
                    if (cartItems.Count == 0)
                    {
                        // If cart is empty, remove the cookie
                        Response.Cookies.Delete("cartItems");
                    }
                    else
                    {
                        // Update cookie with remaining items
                        var cartJson = JsonSerializer.Serialize(cartItems);
                        Response.Cookies.Append("cartItems", cartJson, new CookieOptions
                        {
                            Expires = DateTimeOffset.Now.AddDays(30),
                            Path = "/"
                        });
                    }

                    return new JsonResult(new { success = true, cartItems = cartItems });
                }

                return new JsonResult(new { success = false, message = "Item not found" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }




        [ValidateAntiForgeryToken]
        public IActionResult OnPostGeneratePaymentUrl()
        {
            var customerId = HttpContext.Session.GetString("UserId");
            var userName = HttpContext.Session.GetString("UserName");
            var token = HttpContext.Session.GetString("JWTToken");

            if (token == null)
            {
                TempData["ReturnUrl"] = Url.Page("/CartPages/Cart");
                return new JsonResult(new { success = false, redirectUrl = Url.Page("/Login/Login") });
            }

            var cartJson = HttpContext.Request.Cookies["cartItems"];
            var cartItems = !string.IsNullOrEmpty(cartJson)
                            ? JsonSerializer.Deserialize<List<CartItemDTO>>(cartJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                            : new List<CartItemDTO>();

            if (cartItems == null || !cartItems.Any())
            {
                return new JsonResult(new { success = false, message = "Cart is empty." });
            }

            var totalAmount = (double)cartItems.Sum(item => item.PricePerUnit * item.Quantity);

            // Fetch user details from OData API
            var userApiUrl = $"http://localhost:5077/odata/User?$filter=UserId eq {customerId}";
            string address = string.Empty;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var userResponse = client.GetAsync(userApiUrl).Result;

                if (!userResponse.IsSuccessStatusCode)
                {
                    var errorContent = userResponse.Content.ReadAsStringAsync().Result;
                    return new JsonResult(new { success = false, message = $"Failed to fetch user data: {errorContent}" });
                }

                var responseContent = userResponse.Content.ReadAsStringAsync().Result;
                var oDataResponse = JsonSerializer.Deserialize<ODataResponse<ListUserDTO>>(responseContent);

                if (oDataResponse == null || !oDataResponse.Value.Any())
                {
                    return new JsonResult(new { success = false, message = "Failed to retrieve user address." });
                }

                address = oDataResponse.Value.First().Address;
            }

            var orderRequest = new
            {
                orderDate = DateTime.Now,
                deliveryAddress = address,
                deliveryDate = DateTime.Now.AddDays(3),
                customerId = customerId,
                orderDetails = cartItems.Select(item => new
                {
                    flowerId = item.FlowerId,
                    quantityOrdered = item.Quantity
                }).ToList()
            };

            var orderApiUrl = "http://localhost:5077/api/Order/order";
            int orderId = 0;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var content = new StringContent(JsonSerializer.Serialize(orderRequest), Encoding.UTF8, "application/json");
                var response = client.PostAsync(orderApiUrl, content).Result;

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = response.Content.ReadAsStringAsync().Result;
                    return new JsonResult(new { success = false, message = $"Failed to create order: {errorContent}" });
                }

                var responseContent = response.Content.ReadAsStringAsync().Result;
                var orderResponse = JsonSerializer.Deserialize<OrderResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                orderId = orderResponse.OrderId;
               
            }

            var paymentRequest = new VnPaymentRequestModel
            {
                OrderId = orderId,
                Amount = totalAmount,
                FullName = userName,
                Description = "Payment for Flower Orders"
            };

            var paymentUrl = _paymentService.GeneratePaymentUrl(HttpContext, paymentRequest);
            HttpContext.Response.Cookies.Append("OrderId", orderId.ToString(), new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(1) // Thay đổi thời gian hết hạn nếu cần
            });


            return new JsonResult(new { success = true, paymentUrl });
        }



    }
    public class ODataResponse<T>
    {
        [JsonPropertyName("@odata.context")]
        public string Context { get; set; }

        [JsonPropertyName("value")]
        public List<T> Value { get; set; }
    }
    public class UpdateQuantityRequest
    {
        public string FlowerId { get; set; }
        public int Quantity { get; set; }
    }
    public class DeleteItemRequest
    {
        public string FlowerId { get; set; }
    }
}