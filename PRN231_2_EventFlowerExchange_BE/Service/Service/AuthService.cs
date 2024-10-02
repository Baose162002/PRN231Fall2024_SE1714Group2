using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository.IRepository;
using Service.IService;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        // Constructor: Dependency Injection cho IUserRepository và IConfiguration
        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        // Phương thức xác thực người dùng và tạo token
        public async Task<IActionResult> LoginAsync(User loginUser)
        {
            // Kiểm tra thông tin đăng nhập
            var user = await _userRepository.GetByEmailAsync(loginUser.Email);
            if (user == null || user.Password != loginUser.Password)
            {
                // Trả về lỗi nếu thông tin đăng nhập không hợp lệ
                return new UnauthorizedObjectResult(new { Message = "Invalid email or password" });
            }

            // Tạo danh sách claims chứa thông tin người dùng
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            // Tạo token JWT
            var token = GenerateToken(authClaims);

            // Trả về thông tin đăng nhập thành công
            return new OkObjectResult(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                role = user.Role,
                expiration = token.ValidTo,
                fullName = user.FullName
            });
        }

        // Phương thức private để tạo JWT token
        private JwtSecurityToken GenerateToken(IEnumerable<Claim> authClaims)
        {
            // Tạo khóa bảo mật từ chuỗi secret trong cấu hình
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            // Tạo thông tin xác thực ký số
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Tạo JWT token
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(3), // Token có hiệu lực trong 3 ngày
                claims: authClaims,
                signingCredentials: signIn);

            return token;
        }
    }
}