using AmazonShop.Application.DTOs;
using AmazonShop.Domain.Models;
using AutoMapper;

namespace AmazonShop.API.Profiles
{
    public class AmazonShopMappings : Profile
    {
        public AmazonShopMappings()
        {
            CreateMap<AdminRegisterDTO, Admin>();
            CreateMap<Admin, AdminRegisterDTO>();

            CreateMap<User, UserRegisterDTO>();
            CreateMap<UserRegisterDTO, User>();

            //CreateMap<User, CreateUserLoginDetailsDTO>();
            //CreateMap<CreateUserLoginDetailsDTO, User>();

            CreateMap<Product, ProductDTO>();
            CreateMap<ProductDTO, Product>();

            CreateMap<Product, GetProductDTO>();
            CreateMap<GetProductDTO, Product>();
        }
    }
}
