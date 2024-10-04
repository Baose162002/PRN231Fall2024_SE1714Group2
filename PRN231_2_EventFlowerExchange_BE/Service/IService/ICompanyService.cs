using BusinessObject;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;

namespace Service.IService
{
    public interface ICompanyService
    {
        Task<bool> AddNew(CreateCompanyDTO company);
        Task<CompanyDTO> GetCompanyByID(int id);
        Task<List<CompanyDTO>> GetCompanies();
        Task Update(int id, CreateCompanyDTO updateCompany);
        Task Delete(int id);
        Task<Company> GetCompanyByIdUser(int id);
    }
}
