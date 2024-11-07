using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Linq;
using BusinessObject.DTO.Response;

namespace RBN_FE.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public DashboardModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        public decimal TotalRevenue { get; set; }
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public int TotalQuantity { get; set; }
        public List<OrderChartData> OrderChartData { get; set; }
        public List<UserChartData> UserChartData { get; set; }
        public List<ListOrderDTO> RecentOrders { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Fetch orders
                var ordersResponse = await _httpClient.GetAsync($"{_baseApiUrl}/api/order");
                if (ordersResponse.IsSuccessStatusCode)
                {
                    var orders = await ordersResponse.Content.ReadFromJsonAsync<List<ListOrderDTO>>();
                    OrderChartData = ProcessOrdersForChart(orders);
                    RecentOrders = orders.OrderByDescending(o => DateTime.Parse(o.OrderDate)).Take(5).ToList();
                    TotalOrders = orders.Count;
                    TotalRevenue = orders.Sum(o => o.TotalPrice);
                    TotalQuantity = (int)orders.Sum(o => o.TotalQuantity);
                }

                // Fetch users
                var usersResponse = await _httpClient.GetAsync($"{_baseApiUrl}/api/user");
                if (usersResponse.IsSuccessStatusCode)
                {
                    var users = await usersResponse.Content.ReadFromJsonAsync<List<ListUserDTO>>();
                    TotalUsers = users.Count;
                    UserChartData = ProcessUsersForChart(users);
                }

                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return Page();
            }
        }

        private List<OrderChartData> ProcessOrdersForChart(List<ListOrderDTO> orders)
        {
            return orders
                .GroupBy(o => DateTime.Parse(o.OrderDate).Date)
                .Select(g => new OrderChartData
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Amount = g.Sum(o => o.TotalPrice)
                })
                .OrderBy(d => d.Date)
                .ToList();
        }

        private List<UserChartData> ProcessUsersForChart(List<ListUserDTO> users)
        {
            return users
                .GroupBy(u => u.Role)
                .Select(g => new UserChartData
                {
                    Role = g.Key,
                    Count = g.Count()
                })
                .ToList();
        }
    }

    public class OrderChartData
    {
        public string Date { get; set; }
        public decimal Amount { get; set; }
    }

    public class UserChartData
    {
        public string Role { get; set; }
        public int Count { get; set; }
    }
}