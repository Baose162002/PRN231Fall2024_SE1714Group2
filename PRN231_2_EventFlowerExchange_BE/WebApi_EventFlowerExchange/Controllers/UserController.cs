using BusinessObject.DTO.Request;
using BusinessObject.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Service.IService;
using System;
using System.Threading.Tasks;

namespace WebApi_EventFlowerExchange.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var users = await _userService.GetAllUsers();
            if (users == null || users.Count == 0)
            {
                return NotFound("No users found");
            }
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("register-buyer")]
        public async Task<IActionResult> RegisterBuyer([FromBody] CreateUserDTO createUserDTO)
        {
            try
            {
                var result = await _userService.CreateUser(createUserDTO);
                return Ok(new { message = "Buyer registered successfully", user = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred while registering the buyer: {ex.Message}" });
            }
        }

        [HttpPost("register-seller")]
        public async Task<IActionResult> RegisterSeller([FromBody] RegisterSellerDTO registerSellerDTO)
        {
            try
            {
                var userResponse = await _userService.CreateSeller(registerSellerDTO.CreateCompany, registerSellerDTO.CreateUser);
                return Ok(new
                {
                    Status = "success",
                    Message = "Seller registered successfully",
                    User = userResponse
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"An error occurred while registering the seller: {ex.Message}" });
            }
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO updateUserDTO)
        {
            try
            {
                var result = await _userService.UpdateUser(id, updateUserDTO);
                if (result)
                {
                    return Ok("User updated successfully");
                }
                return BadRequest("Failed to update user");
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUser(id);
            if (result)
            {
                return Ok("User deleted successfully");
            }
            return NotFound("User not found or failed to delete");
        }
    }
}