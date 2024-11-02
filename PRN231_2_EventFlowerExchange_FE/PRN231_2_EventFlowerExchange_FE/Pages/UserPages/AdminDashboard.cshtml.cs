using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;

namespace RBN_FE.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        public decimal TotalRevenue { get; set; }
        public int TotalUsers { get; set; }
        public int TotalFlowers { get; set; }
        public int TotalOrders { get; set; }
        public List<RecentOrder> RecentOrders { get; set; }
        public int TotalBatches { get; set; }

        public IActionResult OnGet()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(token) || userRole != "Admin")
            {
                ModelState.AddModelError(string.Empty, "Unauthorized access. Only Admins can view this page.");
                return RedirectToPage("/Index");
            }

            // Mock data for flower shop
            TotalRevenue = 150000000; // 150M VND
            TotalUsers = 100;
            TotalFlowers = 50;
            TotalOrders = 200;
            TotalBatches = 15;

            RecentOrders = new List<RecentOrder>
            {
                new RecentOrder { Id = 1, Date = DateTime.Now.AddHours(-2), Price = 850000, CustomerName = "Nguyễn Văn A", FlowerName = "Hoa Hồng Đỏ" },
                new RecentOrder { Id = 2, Date = DateTime.Now.AddHours(-4), Price = 1200000, CustomerName = "Trần Thị B", FlowerName = "Hoa Lan Trắng" },
                new RecentOrder { Id = 3, Date = DateTime.Now.AddHours(-8), Price = 750000, CustomerName = "Lê Văn C", FlowerName = "Hoa Cúc Vàng" },
                new RecentOrder { Id = 4, Date = DateTime.Now.AddDays(-1), Price = 2500000, CustomerName = "Phạm Thị D", FlowerName = "Bó Hoa Sinh Nhật" },
                new RecentOrder { Id = 5, Date = DateTime.Now.AddDays(-1), Price = 1500000, CustomerName = "Hoàng Văn E", FlowerName = "Hoa Ly Trắng" }
            };

            return Page();
        }
    }

    public class RecentOrder
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public string CustomerName { get; set; }
        public string FlowerName { get; set; }
    }
}