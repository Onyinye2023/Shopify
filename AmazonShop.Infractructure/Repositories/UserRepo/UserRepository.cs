using AmazonShop.Application.Abstraction.IRepositories.IBaseRepo;
using AmazonShop.Application.Abstraction.IRepositories.IUserRepo;
using AmazonShop.Application.DTOs;
using AmazonShop.Domain.Models;
using AmazonShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonShop.Infractructure.Repositories.UserRepo
{
    public class UserRepository : IBaseRepository<User>, IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(AppDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
       

        public async Task<User> GetByIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<ResponseDTO> RegisterAsync(User userDTO)
        {
            if (userDTO == null)
            {
                throw new ArgumentException("No User to register.", nameof(userDTO));
            }

            try
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(p => p.FirstName == userDTO.FirstName && p.LastName == userDTO.LastName);

                if (existingUser != null)
                {
                    return new ResponseDTO
                    {
                        Success = false,
                        Message = "User already exists."
                    };
                }

                if (userDTO.RoleId != 2)
                {
                    return new ResponseDTO
                    {
                        Success = false,
                        Message = "Invalid role ID. User role must have a RoleId of 2."
                    };
                }

                var role = await _context.Roles.FindAsync(userDTO.RoleId);
                if (role == null)
                {
                    return new ResponseDTO
                    {
                        Success = false,
                        Message = "Invalid role ID."
                    };
                }

                var generatedUserName = GenerateUsername(userDTO.FirstName);
                var generatedPassword = GeneratePassword(userDTO.LastName);

                var user = new User
                {
                    FirstName = userDTO.FirstName,
                    LastName = userDTO.LastName,
                    UserName = generatedUserName,
                    Password = BCrypt.Net.BCrypt.HashPassword(generatedPassword),
                    RoleId = role.RoleId,
                    Role = role
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return new ResponseDTO
                {
                    Success = true,
                    UserName = generatedUserName,
                    Password = generatedPassword,
                    Message = "User registered successfully."
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while registering the user.", ex);
            }
        }
        public async Task<ResponseDTO> UserLogin(UserLoginDTO userLoginDTO)
        {
            if (userLoginDTO == null)
            {
                throw new ArgumentException("Login credentials are empty.", nameof(userLoginDTO));
            }

            try
            {
                var existingUser = await _context.Users
                   .FirstOrDefaultAsync(p => p.UserName == userLoginDTO.UserName && p.RoleId == 2);

                if (existingUser == null)
                {
                    return new ResponseDTO { Success = false, Message = "Invalid username or password." };
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(userLoginDTO.Password, existingUser.Password);
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
        public User GetByUserNameAsync(string userName)
        {
            _logger.LogInformation("GetAdminByUserName called with username: {UserName}", userName);
            var user = _context.Users
                                .Include(a => a.Role)
                                .FirstOrDefault(a => a.UserName == userName);
            if (user == null)
            {
                _logger.LogWarning("User not found for username: {UserName}", userName);
            }
            return user;
        }

        public string GeneratePassword(string lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("Full name cannot be null or empty.", nameof(lastName));
            }

            // Get the first part of the full name, at least 3 characters
            string baseName = new string(lastName.Where(char.IsLetter).ToArray());
            baseName = baseName.Length >= 4 ? baseName.Substring(0, 4) : baseName;

            // Generate a random number (e.g., 100-999)
            Random random = new Random();
            int randomNumber = random.Next(100, 1000);

            // List of special characters to choose from
            char[] specialCharacters = { '!', '@', '#', '$', '%', '^', '&', '*' };
            char specialChar = specialCharacters[random.Next(specialCharacters.Length)];

            // Combine parts to form the password
            string password = $"{baseName}{randomNumber}{specialChar}";

            // Ensure the password is at least 8 characters long
            if (password.Length < 8)
            {
                password = password.PadRight(8, 'X'); // Pad with 'X' if necessary
            }

            return password;
        }

        public string GenerateUsername(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("Full name cannot be null or empty.", nameof(firstName));
            }

            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentException("Full name must contain at least a first name.", nameof(firstName));
            }

            // Generate a random number (e.g., 100-999)
            Random random = new Random();
            int randomNumber = random.Next(100, 1000);

            // Combine first name and random number to form the username
            string username = $"{firstName.ToLower()}{randomNumber}";

            return username;
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }



    }
}
