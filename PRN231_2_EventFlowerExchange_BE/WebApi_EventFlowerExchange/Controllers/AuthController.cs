using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using System.Threading.Tasks;
using Service.IService;
using System.IdentityModel.Tokens.Jwt;
using BusinessObject.Dto.Request;
using AutoMapper;

namespace WebApi_EventFlowerExchange.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest login)
        {
            if (login == null)
            {
                return BadRequest("Invalid login request");
            }

            var user = _mapper.Map<User>(login);
            if (user == null)
            {
                return BadRequest("Mapping failed");
            }

            var result = await _authService.LoginAsync(user);
            return result;
        }

        [HttpGet("validate-token")]
        public IActionResult ValidateToken()
        {
            return Ok("Token is valid");
        }
    }
}