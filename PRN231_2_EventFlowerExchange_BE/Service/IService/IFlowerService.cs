using BusinessObject.DTO.Response;
using BusinessObject.DTO.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IFlowerService
    {
        Task<List<ListFlowerDTO>> GetAllFlowers();
        Task<ListFlowerDTO> GetFlowerById(int id);
        Task Create(CreateFlowerDTO flowerDTO);
        Task Update(UpdateFlowerDTO flowerDTO, int id);
        Task Delete(int id);
    }
}
