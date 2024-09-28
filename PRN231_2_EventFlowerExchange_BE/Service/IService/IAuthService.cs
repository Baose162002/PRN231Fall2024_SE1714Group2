using BusinessObject;
using BusinessObject.Dto.Request;
using BusinessObject.Dto.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(User user);
        Task<UserResponseDto> LoginAsync(LoginUserRequest loginRequest);
    }
}
