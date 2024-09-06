using AmazonShop.Application.Abstraction.IServices.IUserServices;
using AmazonShop.Application.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmazonShop.API.Controllers
{
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;

        public UserController(ILogger<UserController> logger, IUserServices userServices, IMapper mapper)
        {
            _userServices = userServices;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("register_user")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDTO userDTO)
        {
            try
            {
                if (userDTO == null)
                {
                    return BadRequest("User details are missing");
                }

                var response = await _userServices.RegisterUser(userDTO);

                if (!response.Success)
                {
                    return BadRequest(new { Message = response.Message });
                }

                var userResponse = new CreateResponseDTO
                {
                    UserName = response.UserName,
                    Password = response.Password
                };

                return Ok(new { Message = "Your login details are:", AdminDetails = userResponse });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error registering a User: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }



        [HttpPost]
        [Route("user_login")]
        public async Task<IActionResult> UserLogin([FromBody] UserLoginDTO logindto)
        {
            try
            {
                if (!_userServices.IsValidUser(logindto.UserName, logindto.Password))
                {
                    return Unauthorized();
                }

                var result = await _userServices.UserLoginServiceAsync(logindto);

                if (string.IsNullOrEmpty(result))
                {
                    return Unauthorized();
                }

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred while trying to log in.");
            }
        }

        [HttpDelete("delete-user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                var user = await _userServices.DeleteUserAsync(userId);
                if (user == false)
                {
                    return NotFound($"No user found with the ID: {userId}");
                }
                return Ok("User deleted succesfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting user {ex.Message}");
                return StatusCode(500, "Inter server error");
            }
        }




    }
}
