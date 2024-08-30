using AmazonShop.Application.DTOs;
using AmazonShop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonShop.Application.Abstraction.IServices.IUserServices
{
    public interface IUserServices
    {
        Task<ResponseDTO> RegisterUser(UserRegisterDTO userRegisterDTO);
        //Task<ResponseDTO> CreateUserLoginDetails(CreateUserLoginDetailsDTO userLoginDetails, string id);
        //string HashPassword(string password);
        bool IsValidUser(string userName, string password);
        Task<string> UserLoginServiceAsync(UserLoginDTO loginDTO);
        Task<bool> DeleteUserAsync(string userId);


    }
}
