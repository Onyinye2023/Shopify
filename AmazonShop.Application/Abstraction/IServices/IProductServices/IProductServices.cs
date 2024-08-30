using AmazonShop.Application.DTOs;
using AmazonShop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonShop.Application.Abstraction.IServices.IProductServices
{
    public interface IProductServices
    {
        Task<ResponseDTO> AddSingleProductAsync(ProductDTO productdto, string userName);
        Task<ResponseDTO> AddMultipleProductAsync(IEnumerable<ProductDTO> productdto, string userName);
        Task<IEnumerable<GetProductDTO>> GetProductByName(string name);
        Task<IEnumerable<GetProductDTO>> GetProductByCategory(ProductCategory category);

        Task<IEnumerable<GetProductDTO>> GetAllProductsAsync();
        Task<ResponseDTO> DeleteProductAsync(string id, int quantity);
    }
}
