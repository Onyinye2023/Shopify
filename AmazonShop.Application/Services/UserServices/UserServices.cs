using AmazonShop.Application.Abstraction.IRepositories.IBaseRepo;
using AmazonShop.Application.Abstraction.IRepositories.IUserRepo;
using AmazonShop.Application.Abstraction.IServices.IUserServices;
using AmazonShop.Application.DTOs;
using AmazonShop.Domain.Models;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AmazonShop.Application.Services.UserServices
{
    public class UserServices :  IUserServices
    {
        private readonly IBaseRepository<User> _baseRepo;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<UserServices> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public UserServices(IBaseRepository<User> baseRepo, IUserRepository userRepo, ILogger<UserServices> logger,
            IConfiguration configuration, IMapper mapper)
        {
            _baseRepo = baseRepo;
            _logger = logger;
            _mapper = mapper;
            _userRepo = userRepo;
            _configuration = configuration;

        }

        public async Task<ResponseDTO> RegisterUser(UserRegisterDTO userRegisterDTO)
        {
            if (userRegisterDTO == null)
            {
                _logger.LogError("UserDTO is null.");
                return new ResponseDTO { Success = false, Message = "Invalid User details." };
            }
            try
            {
                var userCreated = _mapper.Map<User>(userRegisterDTO);

                if (userCreated == null)
                {
                    _logger.LogError("Mapping of UserDTO to User failed.");
                    return new ResponseDTO { Success = false, Message = "Mapping error." };
                }

                var registerResponse = await _baseRepo.RegisterAsync(userCreated);

                if (!registerResponse.Success)
                {
                    return new ResponseDTO { Success = false, Message = registerResponse.Message };
                }

                return new ResponseDTO
                {
                    Success = true,
                    Message = "User registered successfully.",
                    UserName = registerResponse.UserName,
                    Password = registerResponse.Password
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering a User.");
                return new ResponseDTO { Success = false, Message = "An error occurred while registering a User." };
            }
        }


        public async Task<string> UserLoginServiceAsync(UserLoginDTO loginDTO)
        {
            if (loginDTO == null || string.IsNullOrEmpty(loginDTO.UserName) || string.IsNullOrEmpty(loginDTO.Password))
            {
                return null;
            }

            var user = await _userRepo.UserLogin(loginDTO);

            if (user != null)
            {
                return GenerateJwtToken(loginDTO.UserName);
            }

            return null;
        }

        private string GenerateJwtToken(string userName)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim("Name", userName)
                };


                // Get the user's role
                var role = GetUserRole(userName);

                if (!string.IsNullOrEmpty(role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));

                }
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var jwtSecurityToken = new JwtSecurityToken
                (
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: signinCredentials
                );

                return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public bool IsValidUser(string userName, string password)
        {

            _logger.LogInformation("IsValidUser called with username: {UserName}", userName);

            var user = _baseRepo.GetByUserNameAsync(userName);
            if (user == null)
            {
                _logger.LogWarning("User not found for username: {UserName}", userName);
                return false;
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Invalid password for username: {UserName}", userName);
            }
            return isPasswordValid;
        }

        private string GetUserRole(string userName)
        {
            var user = _baseRepo.GetByUserNameAsync(userName);
            if (user == null)
            {
                throw new ArgumentException("User not found", nameof(userName));
            }

            if (user.Role == null)
            {
                throw new InvalidOperationException($"Role not found for user: {userName}");
            }

            return user.Role.RoleName;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning($"User with ID {userId} not found");
                return false;
            }

            await _userRepo.DeleteAsync(user);
            return true;
        }
    }
}
