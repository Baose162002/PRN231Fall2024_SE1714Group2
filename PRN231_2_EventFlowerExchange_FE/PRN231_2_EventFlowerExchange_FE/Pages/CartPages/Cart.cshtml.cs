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
        private void SaveCartItems(List<CartItemDTO> cartItems)
        {
            var updatedCartJson = JsonSerializer.Serialize(cartItems);
            HttpContext.Response.Cookies.Append("cartItems", updatedCartJson, new CookieOptions { Path = "/" });
        }
        private List<CartItemDTO> GetCartItems()
        {
            var cartJson = HttpContext.Request.Cookies["cartItems"];
            return string.IsNullOrEmpty(cartJson)
                ? new List<CartItemDTO>()
                : JsonSerializer.Deserialize<List<CartItemDTO>>(cartJson);
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
        public IActionResult OnPostDeleteItem(string flowerId)
        {
            // Xóa sản phẩm khỏi giỏ hàng
            var cartItems = GetCartItems(); // Lấy danh sách sản phẩm trong giỏ hàng từ cookie hoặc session
            var itemToRemove = cartItems.FirstOrDefault(item => item.FlowerId == flowerId);
            if (itemToRemove != null)
            {
                cartItems.Remove(itemToRemove);
                // Cập nhật giỏ hàng trong cookie hoặc session
                SaveCartItems(cartItems);
            }

            return new JsonResult(cartItems); // Trả về giỏ hàng đã cập nhật
        }


        [ValidateAntiForgeryToken]
        public IActionResult OnPostGeneratePaymentUrl()
        {
            // Load customerId from session
            var customerId = HttpContext.Session.GetString("UserId");
            var userName = HttpContext.Session.GetString("UserName");
            var token = HttpContext.Session.GetString("JWTToken");

            if (token == null)
            {
                return new JsonResult(new { success = false, redirectUrl = Url.Page("/Login/Login") });
            }

            // Load cart items
            var cartJson = HttpContext.Request.Cookies["cartItems"];
            var cartItems = !string.IsNullOrEmpty(cartJson)
                            ? JsonSerializer.Deserialize<List<CartItemDTO>>(cartJson)
                            : new List<CartItemDTO>();

            if (cartItems == null || !cartItems.Any())
            {
                return new JsonResult(new { success = false, message = "Cart is empty." });
            }

            var totalAmount = (double)cartItems.Sum(item => item.PricePerUnit * item.Quantity);

            // Fetch user details from OData API to get the address
            var userApiUrl = $"http://localhost:5077/odata/User?$filter=UserId eq {customerId}"; // Assuming customerId is the same as UserId
            string address = string.Empty;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var userResponse = client.GetAsync(userApiUrl).Result;

                if (userResponse.IsSuccessStatusCode)
                {
                    var responseContent = userResponse.Content.ReadAsStringAsync().Result;
                    var oDataResponse = JsonSerializer.Deserialize<ODataResponse<ListUserDTO>>(responseContent);

                    // Make sure value is not null and contains data
                    if (oDataResponse != null && oDataResponse.Value.Any())
                    {
                        var user = oDataResponse.Value.First();
                        address = user.Address; // Extract the user's address
                    }
                    else
                    {
                        return new JsonResult(new { success = false, message = "Failed to retrieve user address." });
                    }
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Failed to fetch user data." });
                }
            }

            // Now use the fetched address to create the order
            var orderRequest = new
            {
                orderDate = DateTime.Now,
                deliveryAddress = address,  // Use fetched address
                deliveryDate = DateTime.Now.AddDays(3),
                customerId = customerId,
                // Updated structure for order details
                flowerId = 0, // This will be ignored; included for consistency, may not be needed
                quantityOrdered = 0, // This will be ignored; included for consistency, may not be needed
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

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    var orderResponse = JsonSerializer.Deserialize<OrderResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    orderId = orderResponse.OrderId;
                }
                else
                {
                    var errorContent =  response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}");
                    Console.WriteLine($"Response Content: {errorContent}");
                    return new JsonResult(new { success = false, message = "Failed to create order." });
                }
            }

            // Generate Payment URL using VNPAY service
            var paymentRequest = new VnPaymentRequestModel
            {
                OrderId = orderId,
                Amount = totalAmount,
                FullName = userName,
                Description = "Payment for Flower Orders"
            };

            var paymentUrl = _paymentService.GeneratePaymentUrl(HttpContext, paymentRequest);

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
}