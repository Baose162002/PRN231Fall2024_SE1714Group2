using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using BusinessObject.Dto.Request;
using BusinessObject.Dto.Response;
using Service;
using System.Threading.Tasks;
using Service.IService;

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

            var userResponse = await _authService.LoginAsync(loginRequest);
            if (userResponse != null)
            {
                return Ok(userResponse);
            }

            return Unauthorized(new { Message = "Invalid email or password" });
        }
    }
}