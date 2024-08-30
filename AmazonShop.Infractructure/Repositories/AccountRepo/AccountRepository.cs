using AmazonShop.Application.Abstraction.IRepositories.IAccountRepo;
using AmazonShop.Application.Abstraction.IRepositories.IBaseRepo;
using AmazonShop.Application.DTOs;
using AmazonShop.Domain.Models;
using AmazonShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AmazonShop.Infractructure.Repositories.AccountRepo
{
    public class AccountRepository : IBaseRepository<Admin>, IAccountRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AccountRepository> _logger;

        public AccountRepository(AppDbContext context, ILogger<AccountRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
       

        public async Task<ResponseDTO> RegisterAsync(Admin adminDTO)
        {
            if (adminDTO == null)
            {
                throw new ArgumentException("Admin details are missing.", nameof(adminDTO));
            }

            try
            {
                // Check if the admin already exists based on First Name
                var existingAdmin = await _context.Admins
                    .FirstOrDefaultAsync(p => p.FirstName == adminDTO.FirstName && p.LastName == adminDTO.LastName);

                if (existingAdmin != null)
                {
                    return new ResponseDTO
                    {
                        Success = false,
                        Message = "Admin already exists."
                    };
                }

                // Ensure the role ID is 1
                if (adminDTO.RoleId != 1)
                {
                    return new ResponseDTO
                    {
                        Success = false,
                        Message = "Invalid role ID. Admin role must have a RoleId of 2."
                    };
                }

                var role = await _context.Roles.FindAsync(adminDTO.RoleId);
                if (role == null)
                {
                    _logger.LogInformation("Invalid role ID for Admin: {RoleId}", adminDTO.RoleId);
                   
                    return new ResponseDTO
                    {
                        Success = false,
                        Message = "Invalid role ID."
                    };
                }

                // Create a new Admin entity from DTO
                var admin = new Admin
                {
                    FirstName = adminDTO.FirstName,
                    LastName = adminDTO.LastName,
                    UserName = adminDTO.UserName,
                    Password = BCrypt.Net.BCrypt.HashPassword(adminDTO.Password),
                    RoleId = role.RoleId,
                    Role = role
                };

                _context.Admins.Add(admin);
                await _context.SaveChangesAsync();
               
                return new ResponseDTO
                {
                    Success = true,
                    UserName = adminDTO.UserName,
                    Password = adminDTO.Password,
                    Message = "User registered successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Invalid role ID for Admin: {RoleId}", adminDTO.RoleId);
                throw new Exception("An error occurred during Admin registration.", ex);

            }
        }

        public async Task<ResponseDTO> AdminLogin(AdminLoginDTO adminLoginDTO)
        {
            if (adminLoginDTO == null)
            {
                throw new ArgumentException("Login credentials are empty.", nameof(adminLoginDTO));
            }

            try
            {
                var existingAdmin = await _context.Admins
                    .FirstOrDefaultAsync(p => p.UserName == adminLoginDTO.UserName && p.RoleId == 1);

                if (existingAdmin == null)
                {
                    return new ResponseDTO { Success = false, Message = "Invalid username or password." };
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(adminLoginDTO.Password, existingAdmin.Password);
                if (!isPasswordValid)
                {
                    return new ResponseDTO { Success = false, Message = "Invalid username or password." };
                }

                return new ResponseDTO { Success = true, Message = "Valid credentials" };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred during login.", ex);
            }
        }

        public Admin GetByUserNameAsync(string userName)
        {
            _logger.LogInformation("GetAdminByUserName called with username: {UserName}", userName);
            var admin = _context.Admins
                                .Include(a => a.Role)
                                .FirstOrDefault(a => a.UserName == userName);
            if (admin == null)
            {
                _logger.LogWarning("Admin not found for username: {UserName}", userName);
            }
            return admin;
        }
    }
}
