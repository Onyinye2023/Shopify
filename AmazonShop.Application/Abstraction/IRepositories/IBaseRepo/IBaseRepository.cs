using AmazonShop.Application.DTOs;
using AmazonShop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AmazonShop.Application.Abstraction.IRepositories.IBaseRepo
{
    public interface IBaseRepository <T> where T : class
    {
        Task<ResponseDTO> RegisterAsync(T entity);
        //Task<ResponseDTO> LoginAsync(T entity);
        T GetByUserNameAsync(string userName);


    }
}
