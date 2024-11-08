using AutoMapper;
using BusinessObject;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using BusinessObject.Enum;
using Repository.IRepository;
using Service.IService;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
            if (_userRepository.GetUserById(company.UserId) == null)
            {
            throw new ArgumentNullException(nameof(company.UserId));
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
                UserId = company.UserId,
                TaxNumber = company.TaxNumber,
                PostalCode = company.PostalCode,
                City = company.City,
                Status = EnumList.Status.Active
            };
            await _companyRepository.AddNew(newCompany);
            return true;
        }

        public async Task Delete(int id)
        {
            await _companyRepository.Delete(id);
        }


        public async Task<List<Company>> GetCompanies()
        {
            var companies = await _companyRepository.GetCompanies();
            var dtoCompanies = _mapper.Map<List<Company>>(companies);
            return dtoCompanies;
        }
        public async Task<CompanyDTO> GetCompanyByID(int id)
        {
            var company = await _companyRepository.GetCompanyByID(id);
            var dtoCompany = _mapper.Map<CompanyDTO>(company);
            return dtoCompany;
        }
        public async Task<CompanyDTO> GetCompanyByIdUser(int id)
        {
            var user = await _companyRepository.GetCompanyByIdUser(id);
            var dtoCompany = _mapper.Map<CompanyDTO>(user);
            return dtoCompany;
        }
        public async Task<bool> UpdateCompany(int id, CreateCompanyDTO updateCompanyDTO)
        {
            if (updateCompanyDTO == null)
                throw new ArgumentNullException(nameof(updateCompanyDTO));

            ValidateUpdateInputs(updateCompanyDTO);

            var existingCompany = await _companyRepository.GetCompanyByID(id);
            if (existingCompany == null)
                throw new ArgumentException("Company does not exist.");

            if (existingCompany.CompanyName != updateCompanyDTO.CompanyName && await CheckCompanyNameExist(updateCompanyDTO.CompanyName))
                throw new Exception("The company name already exists. Please use a different name.");

            existingCompany.CompanyName = updateCompanyDTO.CompanyName;
            existingCompany.CompanyDescription = updateCompanyDTO.CompanyDescription;
            existingCompany.CompanyAddress = updateCompanyDTO.CompanyAddress;
            existingCompany.TaxNumber = updateCompanyDTO.TaxNumber;
            existingCompany.PostalCode = updateCompanyDTO.PostalCode;
            existingCompany.Status = EnumList.Status.Active;

            return await _companyRepository.UpdateAsync(existingCompany);
        }


        private void ValidateUpdateInputs(CreateCompanyDTO updateCompanyDTO)
        {
            if (string.IsNullOrWhiteSpace(updateCompanyDTO.CompanyName))
                throw new ArgumentException("Company name cannot be empty.");

            if (!Regex.IsMatch(updateCompanyDTO.TaxNumber, @"^\d{10}$"))
                throw new ArgumentException("Tax number must contain exactly 10 digits.");

            if (!Regex.IsMatch(updateCompanyDTO.PostalCode, @"^\d{5,6}$"))
                throw new ArgumentException("Postal code must contain 5 or 6 digits.");
        }

        private async Task<bool> CheckCompanyNameExist(string companyName)
        {
            var companies = await _companyRepository.GetCompanies();
            return companies.Any(c => c.CompanyName == companyName);
        }

    }
}
