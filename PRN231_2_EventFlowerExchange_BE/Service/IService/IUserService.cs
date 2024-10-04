using BusinessObject.Dto.Response;
using BusinessObject.DTO.Request;
using BusinessObject.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IUserService
    {
        Task<List<ListUserDTO>> GetAllUsers();
        Task<ListUserDTO> GetUserById(int id);
        Task<UserResponseDto> CreateUser(CreateUserDTO createUserDTO);
        Task<bool> UpdateUser(int id, UpdateUserDTO updateUserDTO);
        Task<bool> DeleteUser(int id);
    }
}
