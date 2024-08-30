using AmazonShop.Application.Abstraction.IRepositories.IAccountRepo;
using AmazonShop.Application.Abstraction.IRepositories.IBaseRepo;
using AmazonShop.Application.Abstraction.IServices.IAccountServices;
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

namespace AmazonShop.Application.Services.AccountServices
{
    public class AccountServices : IAccountServices
    {
        private readonly IBaseRepository<Admin> _adminRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly ILogger<AccountServices> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AccountServices(IBaseRepository<Admin> adminRepo,IAccountRepository accountRepo, ILogger<AccountServices> logger,
            IConfiguration configuration, IMapper mapper)
        {
            _adminRepo = adminRepo;
            _logger = logger;
            _mapper = mapper;
            _accountRepo = accountRepo;
            _configuration = configuration;

        }
        public async Task<ResponseDTO> RegisterAdmin(AdminRegisterDTO adminRegisterDTO)
        {
            if (adminRegisterDTO == null)
            {
                _logger.LogError("adminDTO is null.");
                return new ResponseDTO { Success = false, Message = "Invalid Admin details." };
            }
            try
            {
                var passwordCheck = Validator.TryValidateProperty(adminRegisterDTO.Password,
                   new ValidationContext(adminRegisterDTO)
                   {
                       MemberName = nameof(adminRegisterDTO.Password)
                   }, null

                   );
                if (!passwordCheck)
                {
                    return new ResponseDTO { Success = false, Message = "Invalid Password details." };
                }


                var adminCreated = _mapper.Map<Admin>(adminRegisterDTO);


                if (adminCreated == null)
                {
                    _logger.LogError("Mapping of AdminDTO to Admin failed.");
                    return new ResponseDTO { Success = false, Message = "Mapping error." };
                }

                await _adminRepo.RegisterAsync(adminCreated);
                return new ResponseDTO { Success = true, Message = "Admin Registered successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering an Admin.");
                return new ResponseDTO { Success = false, Message = "An error occurred while registering an Admin." };
            }

        }

        public async Task<string> AdminLoginServiceAsync(AdminLoginDTO loginDTO)
        {
            if (loginDTO == null || string.IsNullOrEmpty(loginDTO.UserName) || string.IsNullOrEmpty(loginDTO.Password))
            {
                return null;
            }

            var admin = await _accountRepo.AdminLogin(loginDTO);

            if (admin != null)
            {
                return GenerateJwtToken(loginDTO.UserName);
            }

            return null;
        }
        public bool IsValidUser(string userName, string password)
        {
            _logger.LogInformation("IsValidUser called with username: {UserName}", userName);

            var admin = _adminRepo.GetByUserNameAsync(userName);
            if (admin == null)
            {
                _logger.LogWarning("Admin not found for username: {UserName}", userName);
                return false;
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, admin.Password);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Invalid password for username: {UserName}", userName);
            }
            return isPasswordValid;
        }
        public string HashPassword(string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, 12);
            return hashedPassword;

            // Generate a salt and hash the password
            //string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            //string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

            //return hashedPassword;


            //return BCrypt.Net.BCrypt.HashPassword(password);
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

        private string GetUserRole(string userName)
        {
            var user = _adminRepo.GetByUserNameAsync(userName);
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

    }
}

