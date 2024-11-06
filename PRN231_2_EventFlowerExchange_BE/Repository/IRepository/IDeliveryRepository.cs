using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.Enum.EnumList;

namespace Repository.IRepository
{
    public interface IDeliveryRepository
    {
        Task<List<Delivery>> GetAllDeliveries();
        Task<List<Delivery>> GetDeliveriesByPersonnelId(int id);
        Task Create(Delivery delivery);
        Task Update(Delivery delivery, int id);
        Task Delete(int id);
        Task<List<Order>> GetAllOrderForDelivery();
        Task<bool> ExistsOrder(int orderId);
        Task<bool> ExistsUser(int deliveryPersonnelId);
        Task UpdateOrderStatus(int orderId);
        Task<bool> ExistsDeliveryForOrder(int orderId);
        Task UpdateDeliveryStatus(int deliveryId, int orderId);
    }
}
