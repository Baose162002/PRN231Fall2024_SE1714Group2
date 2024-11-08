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
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<List<CartItemDTO>>(cartJson, options) ?? new List<CartItemDTO>();
            }
            catch (JsonException ex)
            {
                // Log the error (optional)
                Console.WriteLine($"Error deserializing cart items: {ex.Message}");
                return new List<CartItemDTO>(); // Return empty list on error
            }
        }


        public void OnGet()
        {
            CartItems = GetCartItems();
        }

        public IActionResult OnPostUpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            var cartJson = HttpContext.Request.Cookies["cartItems"];
            var cartItems = string.IsNullOrEmpty(cartJson)
                            ? new List<CartItemDTO>()
                            : JsonSerializer.Deserialize<List<CartItemDTO>>(cartJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Log initial state of cart items
            Console.WriteLine("Initial Cart Items: " + JsonSerializer.Serialize(cartItems));

            var item = cartItems.FirstOrDefault(x => x.FlowerId == request.FlowerId);
            if (item != null)
            {
                item.Quantity = request.Quantity;
                Console.WriteLine($"Updated item: {item.FlowerId} to quantity: {item.Quantity}");
            }
            else
            {
                Console.WriteLine($"Item with FlowerId: {request.FlowerId} not found.");
            }

            var updatedCartJson = JsonSerializer.Serialize(cartItems);
            HttpContext.Response.Cookies.Append("cartItems", updatedCartJson, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(30),
                Path = "/"
            });

            // Log the updated cart items
            Console.WriteLine("Updated Cart Items: " + updatedCartJson);

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

            // Tạo request thanh toán
            var paymentRequest = new VnPaymentRequestModel
            {
                OrderId = 0,  // Không tạo order ở đây, giữ giá trị mặc định
                Amount = totalAmount,
                FullName = userName,
                Description = "Payment for Flower Orders"
            };

            var paymentUrl = _paymentService.GeneratePaymentUrl(HttpContext, paymentRequest);
            HttpContext.Response.Cookies.Append("cartItems", cartJson, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(5)
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