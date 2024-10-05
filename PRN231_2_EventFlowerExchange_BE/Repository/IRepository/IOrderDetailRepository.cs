using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IOrderDetailRepository
    {
        Task<List<OrderDetail>> GetAllOrdersDetail();
        Task<OrderDetail> GetOrderDetailById(int id);
        Task Create(OrderDetail order);
        Task Update(OrderDetail order, int id);
        Task Delete(int id);
    }
}
