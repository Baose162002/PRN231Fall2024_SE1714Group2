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
        Task<ListDeliveryDTO> GetDeliveryById(int id);
        Task CreateDelivery(CreateDeliveryDTO createDeliveryDTO);
        Task UpdateDelivery(UpdateDeliveryDTO updateDeliveryDTO, int id);
        Task DeleteDelivery(int id);
    }
}
