using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using BusinessObject.Dto.Request;
using BusinessObject.Dto.Response;
using Service;
using System.Threading.Tasks;
using Service.IService;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace WebApi_EventFlowerExchange.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var token = await _authService.LoginAsync(loginRequest);
            if (token != null)
            {
                return Ok(new { Token = token });
            }

            return Unauthorized(new { Message = "Invalid email or password" });
        }


        [HttpGet("check-token")]
        public IActionResult CheckToken()
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
                return BadRequest("No token provided");

            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            return Ok(new
            {
                Expiration = jwtSecurityToken.ValidTo,
                IsExpired = jwtSecurityToken.ValidTo < DateTime.UtcNow
            });
        }
    }

}