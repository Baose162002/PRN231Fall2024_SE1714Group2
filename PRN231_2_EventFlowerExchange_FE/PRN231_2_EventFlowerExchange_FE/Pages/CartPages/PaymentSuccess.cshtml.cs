using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN231_2_EventFlowerExchange_FE.Service;
using System.Configuration;
using System.Text;
using System.Text.Json;

namespace PRN231_2_EventFlowerExchange_FE.Pages.CartPages
{
    public class PaymentSuccessModel : PageModel
    {
        private readonly PaymentService _paymentService;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;

        public PaymentSuccessModel(PaymentService paymentService, IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _paymentService = paymentService;
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGet()
        {
            var customerId = HttpContext.Session.GetString("UserId");
            var token = HttpContext.Session.GetString("JWTToken");
            try
            {
                var paymentResponse = _paymentService.PaymentExecute(Request.Query);
            
                if (paymentResponse.Success)
                {
                    var cartJson = HttpContext.Request.Cookies["cartItems"];
                    var cartItems = !string.IsNullOrEmpty(cartJson)
                                    ? JsonSerializer.Deserialize<List<CartItemDTO>>(cartJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                                    : new List<CartItemDTO>();

                    if (cartItems == null || !cartItems.Any())
                    {
                        return RedirectToPage("/OrderPages/OrderFailure", new { message = "No items in the cart" });
                    }

                    
                    if (token == null || customerId == null)
                    {
                        return RedirectToPage("/OrderPages/OrderFailure", new { message = "Session expired, please log in again" });
                    }

                    var userApiUrl = $"http://localhost:5077/odata/User?$filter=UserId eq {customerId}";
                    string address = string.Empty;

                    using (var client = _clientFactory.CreateClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        var userResponse = await client.GetAsync(userApiUrl);

                        if (!userResponse.IsSuccessStatusCode)
                        {
                            var errorContent = await userResponse.Content.ReadAsStringAsync();
                            return RedirectToPage("/OrderPages/OrderFailure", new { message = $"Failed to fetch user data: {errorContent}" });
                        }

                        var responseContent = await userResponse.Content.ReadAsStringAsync();
                        var oDataResponse = JsonSerializer.Deserialize<ODataResponse<ListUserDTO>>(responseContent);

                        if (oDataResponse == null || !oDataResponse.Value.Any())
                        {
                            return RedirectToPage("/OrderPages/OrderFailure", new { message = "Failed to retrieve user address." });
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
                        }).ToList(),
                    };

                    var orderApiUrl = "http://localhost:5077/api/Order/order";

                    using (var client = _clientFactory.CreateClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        var content = new StringContent(JsonSerializer.Serialize(orderRequest), Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(orderApiUrl, content);

                        if (!response.IsSuccessStatusCode)
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            return RedirectToPage("/OrderPages/OrderFailure", new { message = $"Failed to create order: {errorContent}" });
                        }

                        HttpContext.Response.Cookies.Delete("cartItems");
                        return RedirectToPage("/OrderPages/OrderSuccess");
                    }
                }

                return RedirectToPage("/OrderPages/OrderFailure");
            }
            catch (Exception ex)
            {
                return RedirectToPage("/OrderPages/OrderFailure", new { message = ex.Message });
            }
        }
    }
}
