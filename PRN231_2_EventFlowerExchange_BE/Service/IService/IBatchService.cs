using BusinessObject;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IBatchService
    {
        Task<List<ListBatchDTO>> GetAllBatch();
        Task<List<ListBatchDTO>> GetAvailableBatchesByFlowerId(int id);

        Task<ListBatchDTO> GetBatchById(int id);
        Task Create(CreateBatchDTO batch);
        Task Update(UpdateBatchDTO updateBatchDTO, int id);
        Task Delete(int id);
    }
}
