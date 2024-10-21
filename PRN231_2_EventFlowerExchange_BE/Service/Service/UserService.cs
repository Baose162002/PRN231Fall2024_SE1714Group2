using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using AutoMapper;
using BusinessObject;
using BusinessObject.Dto.Response;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using BusinessObject.Enum;
using Repository.IRepository;
using Service.IService;

namespace Service.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, ICompanyRepository companyRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<List<User>> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsers();
            return _mapper.Map<List<User>>(users);
        }

        public async Task<ListUserDTO> GetUserById(int id)
        {
            var user = await _userRepository.GetUserById(id);
            return _mapper.Map<ListUserDTO>(user);
        }

        public async Task<UserResponseDto> CreateUser(CreateUserDTO createUserDTO)
        {
            try
            {
          /*      ValidatePhoneNumber(createUserDTO.Phone);*/
                ValidateEmail(createUserDTO.Email);

                if (await CheckEmailExist(createUserDTO.Email))
                {
                    throw new Exception("Email đã tồn tại.");
                }
                if (await CheckPhoneExist(createUserDTO.Phone))
                {
                    throw new Exception("Số điện thoại đã tồn tại.");
                }

                var user = new User
                {
                    FullName = createUserDTO.FullName,
                    Email = createUserDTO.Email,
                    Phone = createUserDTO.Phone,
                    Address = createUserDTO.Address,
                    Role = EnumList.UserRole.Buyer,
                    Password = createUserDTO.Password, 
                    Status = EnumList.Status.Active 
                };

                var success = await _userRepository.AddAsync(user);
                if (!success)
                {
                    throw new Exception("Unable to create user.");
                }

                return _mapper.Map<UserResponseDto>(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserResponseDto> CreateSeller(CreateSellerDTO createSellerDTO, CreateUserDTO createUserDTO)
        {
            try
            {
                ValidatePhoneNumber(createUserDTO.Phone);
                ValidateEmail(createUserDTO.Email);

                if (await CheckEmailExist(createUserDTO.Email))
                {
                    throw new Exception("Email đã tồn tại.");
                }
                if (await CheckPhoneExist(createUserDTO.Phone))
                {
                    throw new Exception("Số điện thoại đã tồn tại.");
                }
                if (await CheckCompanyNameExist(createSellerDTO.CompanyName))
                {
                    throw new Exception("Tên công ty đã tồn tại.");
                }

                var user = new User
                {
                    FullName = createUserDTO.FullName,
                    Email = createUserDTO.Email,
                    Phone = createUserDTO.Phone,
                    Address = createUserDTO.Address,
                    Role = EnumList.UserRole.Seller,
                    Password = createUserDTO.Password,  
                    Status = EnumList.Status.Active
                };

                var success = await _userRepository.AddAsync(user);
                if (!success)
                {
                    throw new Exception("Không thể tạo người dùng.");
                }

                var company = new Company
                {
                    CompanyName = createSellerDTO.CompanyName,
                    CompanyAddress = createSellerDTO.CompanyAddress,
                    CompanyDescription = createSellerDTO.CompanyDescription,
                    TaxNumber = createSellerDTO.TaxNumber,
                    City = createSellerDTO.City,
                    PostalCode = createSellerDTO.PostalCode,
                    UserId = user.UserId,
                    Status = EnumList.Status.Active
                };
                var companySuccess = await _companyRepository.AddNew(company);
                if (!companySuccess)
                {
                    throw new Exception("Không thể tạo công ty.");
                }

                return _mapper.Map<UserResponseDto>(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateUser(int id, UpdateUserDTO updateUserDTO)
        {
            if (updateUserDTO == null)
                throw new ArgumentNullException(nameof(updateUserDTO));

            var existingUser = await _userRepository.GetUserById(id);
            if (existingUser == null)
                throw new ArgumentException("User not found");

            // Update user fields from DTO
            existingUser.FullName = updateUserDTO.FullName;
            existingUser.Email = updateUserDTO.Email;
            existingUser.Phone = updateUserDTO.Phone;
            existingUser.Address = updateUserDTO.Address;
            existingUser.Role = updateUserDTO.Role;

            // Always set status to Active
            existingUser.Status = EnumList.Status.Active;

            return await _userRepository.UpdateAsync(existingUser);
        }


        public async Task<bool> DeleteUser(int id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        private async Task<bool> CheckEmailExist(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user != null;
        }

        private async Task<bool> CheckPhoneExist(string phone)
        {
            var users = await _userRepository.GetAllUsers();
            return users.Any(u => u.Phone == phone);
        }

        private async Task<bool> CheckCompanyNameExist(string companyName)
        {
            var companies = await _companyRepository.GetCompanies();
            return companies.Any(c => c.CompanyName == companyName);
        }

        private void ValidatePhoneNumber(string phone)
        {
            if (!Regex.IsMatch(phone, @"^\d{10}$"))
            {
                throw new Exception("Số điện thoại phải có đúng 10 chữ số.");
            }
        }

        private void ValidateEmail(string email)
        {
            if (!email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Email phải có đuôi @gmail.com.");
            }
        }
    }
}