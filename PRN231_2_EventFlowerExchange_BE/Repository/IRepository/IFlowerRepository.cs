using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IFlowerRepository
    {
        Task<List<Flower>> GetAllFlowers();
        Task<Flower> GetFlowerById(int id);
        Task Create(Flower flower);
        Task Update(Flower flower, int id);
        Task Delete(int id);
    }
}
