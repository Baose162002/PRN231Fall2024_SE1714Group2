using BusinessObject;
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
        Task<Order> GetOrderById(int id);
        Task Create(Order order);
        Task Update(Order order, int id);
        Task Delete(int id);
    }
}
