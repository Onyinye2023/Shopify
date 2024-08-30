using AmazonShop.Application.DTOs;
using AmazonShop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonShop.Application.Abstraction.IRepositories.IUserRepo
{
    public interface IUserRepository
    {
        //Task RegisterUser(User user);
        //Task CreateUserLoginDetails(User user, string id);
        Task<User> GetByIdAsync(string id);
        //User GetUserByUserName(string userName);
        Task<ResponseDTO> UserLogin(UserLoginDTO userLoginDTO);
        string GeneratePassword(string fullName);
        string GenerateUsername(string fullName);
        Task DeleteAsync(User user);
        Task SaveChangesAsync();

    }
}
