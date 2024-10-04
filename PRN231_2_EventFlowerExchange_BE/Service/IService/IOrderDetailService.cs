using BusinessObject;
using BusinessObject.DTO.Response;
using BusinessObject.DTO.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IOrderDetailService
    {
        Task<List<ListOrderDetailDTO>> GetAllOrderDetail();
        Task<ListOrderDetailDTO> GetOrderDetailById(int id);
        //Task Create(CreateOrderDetailDTO order);
        //Task Update(UpdateOrderDetailDTO updateOrderDTO, int id);
        Task Delete(int id);
    }
}
