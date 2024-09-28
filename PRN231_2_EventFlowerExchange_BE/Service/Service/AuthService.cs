using BusinessObject;
using BusinessObject.Dto.Request;
using BusinessObject.Dto.Response;
using Repository.IRepository;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResponseDto> LoginAsync(LoginUserRequest loginRequest)
        {
            var user = await _userRepository.GetByEmailAsync(loginRequest.Email);
            if (user != null && user.Password == loginRequest.Password)
            {
                return new UserResponseDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Address = user.Address,
                    Role = user.Role
                };
            }
            return null;
        }

        public Task<bool> RegisterAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}
