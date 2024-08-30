using AmazonShop.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonShop.Application.Abstraction.IServices.IAccountServices
{
    public interface IAccountServices
    {
        Task<ResponseDTO> RegisterAdmin(AdminRegisterDTO adminRegisterDTO);
        string HashPassword(string password);
        bool IsValidUser(string userName, string password);
        Task<string> AdminLoginServiceAsync(AdminLoginDTO logindto);

    }
}
