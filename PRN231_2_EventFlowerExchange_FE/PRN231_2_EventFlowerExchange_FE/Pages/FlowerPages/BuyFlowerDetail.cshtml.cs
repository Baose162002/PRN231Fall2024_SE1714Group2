using Azure.Core;
using BusinessObject;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN231_2_EventFlowerExchange_FE.Service;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PRN231_2_EventFlowerExchange_FE.Pages.FlowerPages
{
    public class BuyFlowerDetailModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly PaymentService _paymentService;

        public BuyFlowerDetailModel(IConfiguration configuration, HttpClient httpClient, PaymentService paymentService)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _paymentService = paymentService;
        }

        public ListFlowerDTO Flower { get; set; }
        public List<ListReviewDTO> Reviews { get; set; }

        [BindProperty]
        public CreateReviewDTO NewReview { get; set; }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer");
            var baseApiUrl = _configuration["ApiSettings:BaseUrl"];

            // Fetch flower details
            var flowerResponse = await _httpClient.GetAsync($"{baseApiUrl}/api/flower/getby/{id}");
            if (flowerResponse.IsSuccessStatusCode)
            {
                var jsonContent = await flowerResponse.Content.ReadAsStringAsync();
                Flower = JsonSerializer.Deserialize<ListFlowerDTO>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to load flower data.");
                return Page();
            }

            // Fetch reviews
            if (Flower != null)
            {
                var reviewResponse = await _httpClient.GetAsync($"{baseApiUrl}/api/review/flower/{id}");
                if (reviewResponse.IsSuccessStatusCode)
                {
                    var jsonContent = await reviewResponse.Content.ReadAsStringAsync();
                    Reviews = JsonSerializer.Deserialize<List<ListReviewDTO>>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
               
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSubmitReviewAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Fetch the flower details again for the POST request
            var baseApiUrl = _configuration["ApiSettings:BaseUrl"];
            var flowerResponse = await _httpClient.GetAsync($"{baseApiUrl}/api/flower/getby/{id}");
            if (flowerResponse.IsSuccessStatusCode)
            {
                var jsonContent = await flowerResponse.Content.ReadAsStringAsync();
                Flower = JsonSerializer.Deserialize<ListFlowerDTO>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
               
                return Page();
            }

            // Set the flower ID and customer ID
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                ModelState.AddModelError(string.Empty, "You must be logged in to submit a review.");
                return Page();
            }

            NewReview.CustomerId = int.Parse(userIdString);
            NewReview.FlowerId = Flower.FlowerId;

            // Send the review to the backend API
            var jsonContentReview = JsonSerializer.Serialize(NewReview);
            var content = new StringContent(jsonContentReview, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{baseApiUrl}/api/review", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage(new { id = Flower.FlowerId });
            }
            else
            {
                // Extract error message from the response content
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Failed to submit review. Error: {errorContent}");
                return Page();
            }
        }


        public async Task<IActionResult> OnPostUpdateReviewAsync(int reviewId, int flowerId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var baseApiUrl = _configuration["ApiSettings:BaseUrl"];

            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                ModelState.AddModelError(string.Empty, "You must be logged in to update a review.");
                return Page();
            }

            var updateReview = new UpdateReviewDTO
            {
                Rating = NewReview.Rating,
                Feedback = NewReview.Feedback,
                ReviewDate = DateTime.Now
            };

            var jsonContentReview = JsonSerializer.Serialize(updateReview);
            var content = new StringContent(jsonContentReview, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{baseApiUrl}/api/review/{reviewId}", content);

            if (response.IsSuccessStatusCode)
            {
                // Chuyển hướng sau khi cập nhật thành công
                return RedirectToPage(new { id = flowerId });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to update review.");
                return Page();
            }
        }


        public async Task<IActionResult> OnPostDeleteReviewAsync(int reviewId, int flowerId)
        {
            var baseApiUrl = _configuration["ApiSettings:BaseUrl"];

            // Xóa review
            var response = await _httpClient.DeleteAsync($"{baseApiUrl}/api/review/{reviewId}");

            if (response.IsSuccessStatusCode)
            {
                // Lấy lại thông tin của Flower để đảm bảo không bị null
                var flowerResponse = await _httpClient.GetAsync($"{baseApiUrl}/api/flower/getby/{flowerId}");
                if (flowerResponse.IsSuccessStatusCode)
                {
                    var jsonContent = await flowerResponse.Content.ReadAsStringAsync();
                    Flower = JsonSerializer.Deserialize<ListFlowerDTO>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                return RedirectToPage(new { id = flowerId });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to delete review.");
                return Page();
            }
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
            var baseApiUrl = _configuration["ApiSettings:BaseUrl"];

            var response = await _httpClient.GetAsync($"{baseApiUrl}/api/Flower/GetBy/{flowerId}");
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

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostGeneratePaymentUrl(int flowerId)
        {
            var customerId = HttpContext.Session.GetString("UserId");
            var userName = HttpContext.Session.GetString("UserName");
            var token = HttpContext.Session.GetString("JWTToken");
            var baseApiUrl = _configuration["ApiSettings:BaseUrl"];
            HttpContext.Session.SetString("FlowerId", flowerId.ToString());

            if (token == null)
            {
                // Chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
                TempData["ReturnUrl"] = Url.Page("/FlowerDetails");
                return new JsonResult(new { success = false, redirectUrl = Url.Page("/Login/Login") });
            }

            // Gọi API để lấy thông tin sản phẩm dựa vào flowerId
            var flowerResponse = await _httpClient.GetAsync($"{baseApiUrl}/api/flower/getby/{flowerId}");

            if (!flowerResponse.IsSuccessStatusCode)
            {
                // Trường hợp không thành công, trả về thông báo lỗi
                return new JsonResult(new { success = false, message = "Failed to retrieve flower details." });
            }

            var flowerContent = await flowerResponse.Content.ReadAsStringAsync();
            var flower = JsonSerializer.Deserialize<ListFlowerDTO>(flowerContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (flower == null)
            {
                return new JsonResult(new { success = false, message = "Flower not found." });
            }

            // Tính tổng tiền cho số lượng 1
            double totalAmount = (double)(flower.PricePerUnit * 1); // Số lượng mặc định là 1

            // Tạo request thanh toán cho VNPAY
            var paymentRequest = new VnPaymentRequestModel
            {
                OrderId = flowerId,  // Sử dụng flowerId hoặc tạo mã hóa đơn riêng
                Amount = totalAmount,
                FullName = userName,
                Description = $"Payment for {flower.Name} (1 item)"
            };

            var paymentUrl = _paymentService.GeneratePaymentUrl(HttpContext, paymentRequest);

            return Redirect(paymentUrl);
        }


    }

    public class AddToCartRequest
    {
        public string FlowerId { get; set; }
    }

}