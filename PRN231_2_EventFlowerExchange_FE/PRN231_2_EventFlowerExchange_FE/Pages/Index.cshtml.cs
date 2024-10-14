using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

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

        public async Task OnGetAsync()
        {
            // Lấy token từ session (nếu cần)
            var token = HttpContext.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Gọi API để lấy danh sách hoa
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/api/Flower/GetAll");

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                Flowers = await response.Content.ReadFromJsonAsync<List<ListFlowerDTO>>(options);
            }
            else
            {
                Flowers = new List<ListFlowerDTO>();
                ModelState.AddModelError(string.Empty, "Không thể tải danh sách hoa.");
            }
        }

        // Chuyển phương thức thành bất đồng bộ (async)
        public async Task<IActionResult> OnPostAddToCartAsync(string flowerId)
        {
            // Gọi phương thức bất đồng bộ để lấy thông tin hoa
            var flower = await GetFlowerById(flowerId);

            if (flower != null)
            {
                var cartItem = new CartItemDTO
                {
                    FlowerId = flower.FlowerId.ToString(),
                    Name = flower.Name,
                    Description = flower.Description,
                    PricePerUnit = flower.PricePerUnit,
                    Image = flower.Image,
                    Quantity = 1 // Quản lý số lượng nếu cần
                };

                AddToCart(cartItem);
            }

            return RedirectToPage(); // Quay lại trang hiện tại
        }

        // Phương thức để lấy thông tin hoa bất đồng bộ
        private async Task<ListFlowerDTO> GetFlowerById(string flowerId)
        {
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/Flower/{flowerId}");
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return await response.Content.ReadFromJsonAsync<ListFlowerDTO>(options);
            }

            return null;
        }

        public void AddToCart(CartItemDTO flower)
        {
            // Lấy giỏ hàng hiện tại từ cookie (nếu có)
            var cartJson = HttpContext.Request.Cookies["cartItems"];
            List<CartItemDTO> cartItems = string.IsNullOrEmpty(cartJson)
                ? new List<CartItemDTO>()
                : JsonSerializer.Deserialize<List<CartItemDTO>>(cartJson);

            // Kiểm tra xem hoa đã tồn tại trong giỏ hàng chưa
            var existingItem = cartItems.FirstOrDefault(x => x.FlowerId == flower.FlowerId);
            if (existingItem != null)
            {
                // Nếu đã có, tăng số lượng
                existingItem.Quantity += 1;
            }
            else
            {
                // Nếu không có, thêm hoa mới vào giỏ hàng với số lượng 1
                flower.Quantity = 1;
                cartItems.Add(flower);
            }

            // Cập nhật cookie với danh sách giỏ hàng mới
            var options = new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) };
            HttpContext.Response.Cookies.Append("cartItems", JsonSerializer.Serialize(cartItems), options);
        }

    }
}
