using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IBatchRepository
    {
        Task<List<Batch>> GetAllBatch();
        Task<Batch> GetBatchById(int id);
        Task Create(Batch batch);
        Task Update(Batch batch, int id);
        Task Delete(int id);
        Task<Flower> GetFlowerById(int id);

        Task<List<Flower>> GetFlowersBySimilarTypeAndColorAndEarliestBatch(int flowerId);
        Task UpdateBatch(Batch batch);
        Task CheckAndUpdateBatchStatus();
    }
}
