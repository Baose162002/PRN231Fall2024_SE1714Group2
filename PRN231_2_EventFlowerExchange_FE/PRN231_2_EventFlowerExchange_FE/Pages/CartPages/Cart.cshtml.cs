using BusinessObject.DTO.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace PRN231_2_EventFlowerExchange_FE.Pages.CartPages
{
    public class CartModel : PageModel
    {
        public List<CartItemDTO> CartItems { get; set; } = new List<CartItemDTO>();

        public void OnGet()
        {
            // Lấy dữ liệu giỏ hàng từ cookie
            var cartJson = HttpContext.Request.Cookies["cartItems"];
            if (!string.IsNullOrEmpty(cartJson))
            {
                CartItems = JsonSerializer.Deserialize<List<CartItemDTO>>(cartJson);
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult OnPostUpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            // Lấy giỏ hàng từ cookie
            var cartJson = HttpContext.Request.Cookies["cartItems"];
            var cartItems = string.IsNullOrEmpty(cartJson) ? new List<CartItemDTO>() : JsonSerializer.Deserialize<List<CartItemDTO>>(cartJson);

            // Tìm sản phẩm trong giỏ hàng và cập nhật số lượng
            var item = cartItems.FirstOrDefault(x => x.FlowerId == request.FlowerId);
            if (item != null)
            {
                item.Quantity = request.Quantity; // Cập nhật số lượng
            }

            // Lưu lại giỏ hàng vào cookie
            var updatedCartJson = JsonSerializer.Serialize(cartItems);
            HttpContext.Response.Cookies.Append("cartItems", updatedCartJson, new CookieOptions { Path = "/" });

            return new JsonResult(cartItems); // Trả lại giỏ hàng đã cập nhật
        }

        [ValidateAntiForgeryToken]
        public IActionResult OnPostOrder()
        {
            // Xử lý đặt hàng ở đây
            // Ví dụ: lưu đơn hàng vào database

            // Xóa giỏ hàng sau khi đặt hàng thành công
            HttpContext.Response.Cookies.Delete("cartItems");

            return new OkResult();
        }
    }

    public class UpdateQuantityRequest
    {
        public string FlowerId { get; set; }
        public int Quantity { get; set; }
    }
}