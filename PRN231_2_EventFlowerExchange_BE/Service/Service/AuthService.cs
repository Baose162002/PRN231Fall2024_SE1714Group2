using BusinessObject;
using BusinessObject.Enum;
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

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<IActionResult> LoginAsync(User loginUser)
        {
            // Retrieve user by email
            var user = await _userRepository.GetByEmailAsync(loginUser.Email);

            // Check if user exists or password is incorrect
            if (user == null || user.Password != loginUser.Password)
            {
                return new UnauthorizedObjectResult(new { Message = "Invalid email or password" });
            }

            // Check if the user's status is inactive
            if (user.Status == EnumList.Status.Inactive)  
            {
                return new UnauthorizedObjectResult(new { Message = "Account does not exist or is inactive" });
            }


            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = GenerateToken(authClaims);

            return new OkObjectResult(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                role = user.Role,
                expiration = token.ValidTo,
                fullName = user.FullName,
                userId = user.UserId
            });
        }

        private JwtSecurityToken GenerateToken(IEnumerable<Claim> authClaims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(3), 
                claims: authClaims,
                signingCredentials: signIn);

            return token;
        }
    }
}