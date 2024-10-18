using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN231_2_EventFlowerExchange_FE.Service;
using System.Text;
using System.Text.Json;

namespace PRN231_2_EventFlowerExchange_FE.Pages.CartPages
{
    public class PaymentSuccessModel : PageModel
    {
        private readonly PaymentService _paymentService;

        public PaymentSuccessModel(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<IActionResult> OnGet()
        {
            try
            {
                var paymentResponse =  _paymentService.PaymentExecute(Request.Query);

                if (paymentResponse.Success)
                {
                    // Prepare payment confirmation request
                    var paymentApiUrl = "http://localhost:5077/api/Payment";
                    var paymentRequest = new
                    {
                        paymentStatus = "Completed",
                        amountPaid = paymentResponse.Amount,
                        paymentDate = DateTime.Now,
                        orderId = paymentResponse.OrderId
                    };

                    using (var client = new HttpClient())
                    {
                        var content = new StringContent(JsonSerializer.Serialize(paymentRequest), Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(paymentApiUrl, content);

                        if (!response.IsSuccessStatusCode)
                        {
                            return new JsonResult(new { success = false, message = "Failed to update payment status." });
                        }
                    }

                    // Clear cart items only if payment is successful
                    HttpContext.Response.Cookies.Delete("cartItems");

                    return new JsonResult(new { success = true, redirectUrl = Url.Page("/OrderPages/OrderSuccess") });
                }
                else
                {
                    // Handle payment failure
                    var orderId = paymentResponse.OrderId;

                    // Prepare payment failure request
                    var paymentApiUrl = "http://localhost:5077/api/Payment";
                    var paymentRequest = new
                    {
                        paymentStatus = "Failed",
                        amountPaid = paymentResponse.Amount,
                        paymentDate = DateTime.Now,
                        orderId = orderId
                    };

                    using (var client = new HttpClient())
                    {
                        var content = new StringContent(JsonSerializer.Serialize(paymentRequest), Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(paymentApiUrl, content);

                        if (!response.IsSuccessStatusCode)
                        {
                            return new JsonResult(new { success = false, redirectUrl = Url.Page("/OrderPages/OrderFailure") });
                        }
                    }

                    return RedirectToPage("/OrderPages/OrderFailure");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return RedirectToPage("/OrderPages/OrderFailure");
            }
        }
    }
}
