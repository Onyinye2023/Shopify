using AmazonShop.Application.DTOs;
using AmazonShop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonShop.Application.Abstraction.IRepositories.IAccountRepo
{
    public interface IAccountRepository
    {
        //Task RegisterAdmin(Admin admin);
        Task<ResponseDTO> AdminLogin(AdminLoginDTO adminLoginDTO);
        //Admin GetAdminByUserName(string userName);

    }
}
