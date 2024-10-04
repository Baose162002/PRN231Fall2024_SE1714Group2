using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface ICompanyRepository
    {
        Task<bool> AddNew(Company company);
        Task<Company> GetCompanyByID(int id);
        Task<List<Company>> GetCompanies();
        Task Update(int id, Company updateCompany);
        Task Delete(int id);
        Task<Company> GetCompanyByIdUser(int id);
    }
}
