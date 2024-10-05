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

        public IActionResult OnPostUpdateQuantity(string flowerId, int quantity)
        {
            // Lấy giỏ hàng hiện tại từ cookie
            var cartJson = HttpContext.Request.Cookies["cartItems"];
            if (string.IsNullOrEmpty(cartJson)) return RedirectToPage();

            var cartItems = JsonSerializer.Deserialize<List<CartItemDTO>>(cartJson);

            // Tìm sản phẩm trong giỏ hàng và cập nhật số lượng
            var cartItem = cartItems.FirstOrDefault(x => x.FlowerId == flowerId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;  // Cập nhật số lượng
                if (cartItem.Quantity <= 0)
                {
                    cartItems.Remove(cartItem);  // Xóa khỏi giỏ hàng nếu số lượng <= 0
                }
            }

            // Cập nhật lại cookie
            var options = new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) };
            HttpContext.Response.Cookies.Append("cartItems", JsonSerializer.Serialize(cartItems), options);

            return RedirectToPage(); // Refresh trang Cart
        }

        public IActionResult OnPostOrder()
        {
            // Xử lý khi người dùng nhấn nút "Order" và chuyển đến trang Order
            return RedirectToPage("/Order");
        }
    }
}
