using BusinessObject;
using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly FlowerShopContext _context;

        public UserRepository()
        {
            _context ??= new FlowerShopContext();
        }

        public async Task<bool> AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            using (var _context = new FlowerShopContext())
            {
                // Tìm khách hàng theo CustomerId
                var customer = await _context.Users.FirstOrDefaultAsync(c => c.UserId == id);

                // Trả về khách hàng hoặc null nếu không tìm thấy
                return customer;
            }
        }
    }
}
    