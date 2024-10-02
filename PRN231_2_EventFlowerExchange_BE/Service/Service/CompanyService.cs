using AutoMapper;
using BusinessObject;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using Repository.IRepository;
using Service.IService;

namespace Service.Service
{
    public class CompanyService : ICompanyService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public CompanyService(IMapper mapper, ICompanyRepository companyRepository,IUserRepository userRepository)
        {
            _mapper = mapper;
            _companyRepository = companyRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> AddNew(CreateCompanyDTO company)
        {
            if (company == null) throw new ArgumentNullException(nameof(company));
            if (_userRepository.GetUserById(company.SellerId) == null)
            {
            throw new ArgumentNullException(nameof(company.SellerId));
            }
            if (string.IsNullOrEmpty(company.CompanyName))
            {
                throw new ArgumentException("All fieds must be filled");
            }
            if (string.IsNullOrEmpty(company.CompanyAddress))
            {
                throw new ArgumentException("All fieds must be filled");
            }
            if (string.IsNullOrEmpty(company.CompanyDescription))
            {
                throw new ArgumentException("All fieds must be filled");
            }
            var newCompany = new Company
            {
                CompanyName = company.CompanyName,
                CompanyDescription = company.CompanyDescription,
                CompanyAddress = company.CompanyAddress,
                CompanyId = company.CompanyId,
                SellerId = company.SellerId
            };
            await _companyRepository.AddNew(newCompany);
            return true;
        }

        public async Task Delete(int id)
        {
            await _companyRepository.Delete(id);
        }

        public async Task<List<CompanyDTO>> GetCompanies()
        {
            var companies = await _companyRepository.GetCompanies();
            var dtoCompanies = _mapper.Map<List<CompanyDTO>>(companies);
            return dtoCompanies;
        }

        public async Task<CompanyDTO> GetCompanyByID(int id)
        {
            var company = await _companyRepository.GetCompanyByID(id);
            var dtoCompany = _mapper.Map<CompanyDTO>(company);
            return dtoCompany;
        }

        public async Task Update(int id, CreateCompanyDTO company)
        {
            if (company == null) throw new ArgumentNullException(nameof(company));
            if (_userRepository.GetUserById(company.SellerId) == null)
            {
                throw new ArgumentNullException(nameof(company.SellerId));
            }
            if (string.IsNullOrEmpty(company.CompanyName))
            {
                throw new ArgumentException("All fieds must be filled");
            }
            if (string.IsNullOrEmpty(company.CompanyAddress))
            {
                throw new ArgumentException("All fieds must be filled");
            }
            if (string.IsNullOrEmpty(company.CompanyDescription))
            {
                throw new ArgumentException("All fieds must be filled");
            }
            var existingCompany = await _companyRepository.GetCompanyByID(id);
            if (existingCompany == null)
            {
                throw new ArgumentNullException(nameof(company));
            }
            existingCompany.CompanyName = company.CompanyName;
            existingCompany.CompanyDescription = company.CompanyDescription;
            existingCompany.CompanyAddress = company.CompanyAddress;
            await _companyRepository.Update(id, existingCompany);
        }
    }
}
