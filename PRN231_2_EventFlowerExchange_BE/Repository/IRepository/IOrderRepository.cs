using BusinessObject;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllOrders();
        Task<List<Order>> GetAllOrdersByUserId(int id);

        Task<List<Order>> GetOrdersBySellerBatch(int sellerId);
        Task<Order> GetOrderById(int id);
        Task Create(Order order);
        Task Update(Order order, int id);
        Task Delete(int id);
        Order UpdateOrderStatusAsync(int id);

        Task<List<Order>> SearchOrders(OrderSearchDTO searchCriteria);
        Task UpdateStatus(Order order, int id);
    }
}
