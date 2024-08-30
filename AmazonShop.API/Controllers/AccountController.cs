using AmazonShop.Application.Abstraction.IServices.IAccountServices;
using AmazonShop.Application.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AmazonShop.API.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountServices _accountServices;
        private readonly ILogger<AccountController> _logger;
        private readonly IMapper _mapper;

    public AccountController(ILogger<AccountController> logger, IAccountServices accountServices, IMapper mapper)
    {
        _accountServices = accountServices;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] AdminRegisterDTO adminDTO)
    {
        try
        {
            if (adminDTO == null)
            {
                return BadRequest("Admin details is missing");
            }

            var response = await _accountServices.RegisterAdmin(adminDTO);

            if (!response.Success)
            {
                return BadRequest(new { Message = response.Message });
            }

                var adminResponse = new CreateResponseDTO
                {
                    UserName = adminDTO.UserName,
                    Password = adminDTO.Password
                };

                return Ok(new { Message = "Your login details are:", AdminDetails = adminResponse });
            }
            catch (Exception ex)
        {
            _logger.LogError($"Error registering an Admin: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

        [HttpPost]
        [Route("admin-login")]
        public async Task<IActionResult> AdminLogin([FromBody] AdminLoginDTO logindto)
        {
            try
            {
                if (!_accountServices.IsValidUser(logindto.UserName, logindto.Password))
                {
                    return Unauthorized();
                }

                var result = await _accountServices.AdminLoginServiceAsync(logindto);

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

    }
}
