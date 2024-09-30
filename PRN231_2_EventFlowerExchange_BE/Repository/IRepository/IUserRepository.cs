using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IUserRepository
    {
        Task<bool> AddAsync(User user);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetUserByIdAsync(int id);
    }
}
