using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IDeliveryService
    {
        Task<List<ListDeliveryDTO>> GetAllDeliveries();
        Task<List<ListDeliveryDTO>> GetDeliveryById(int id);
        Task CreateDelivery(CreateDeliveryDTO createDeliveryDTO);
        Task UpdateDelivery(UpdateDeliveryDTO updateDeliveryDTO, int id);
        Task DeleteDelivery(int id);
        Task<List<ListOrderForDeliveryDTO>> GetAllOrdersAsync();
        Task UpdateDeliveryStatus(int deliveryId, int orderId);
    }
}
