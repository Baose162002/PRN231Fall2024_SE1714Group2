using AutoMapper;
using BusinessObject;
using BusinessObject.Dto.Response;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using BusinessObject.Enum;
using Repository.IRepository;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<List<ListUserDTO>> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsers();
            return _mapper.Map<List<ListUserDTO>>(users);
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
                    Role = EnumList.UserRole.Buyer, // Đảm bảo role là Buyer
                    Password = createUserDTO.Password // Lưu ý: Nên hash mật khẩu trước khi lưu
                };

                var success = await _userRepository.AddAsync(user);
                if (!success)
                {
                    throw new Exception("Không thể tạo người dùng.");
                }


                return _mapper.Map<UserResponseDto>(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo người dùng: {ex.Message}");
            }
        }

        public async Task<bool> UpdateUser(int id, UpdateUserDTO updateUserDTO)
        {
            if (updateUserDTO == null)
                throw new ArgumentNullException(nameof(updateUserDTO));

            var existingUser = await _userRepository.GetUserById(id);
            if (existingUser == null)
                throw new ArgumentException("User not found");

            _mapper.Map(updateUserDTO, existingUser);
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
    }
}