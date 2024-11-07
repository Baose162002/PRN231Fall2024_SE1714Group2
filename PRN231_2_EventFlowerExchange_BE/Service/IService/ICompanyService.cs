using BusinessObject;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;

namespace Service.IService
{
    public interface ICompanyService
    {
        Task<bool> AddNew(CreateCompanyDTO company);
        Task<CompanyDTO> GetCompanyByID(int id);
        Task<List<Company>> GetCompanies();
        Task<bool> UpdateCompany(int id, CreateCompanyDTO updateCompanyDTO);
        Task Delete(int id);
        Task<CompanyDTO> GetCompanyByIdUser(int id);
    }
}
