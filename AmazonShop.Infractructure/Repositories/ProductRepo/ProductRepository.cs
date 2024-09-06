using AmazonShop.Application.Abstraction.IRepositories.IProductRepo;
using AmazonShop.Domain.Enums;
using AmazonShop.Domain.Models;
using AmazonShop.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace AmazonShop.Infractructure.Repositories.ProductRepo
{
    public class ProductRepository : IProductRepository<Product>
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task AddMultipleProductAsync(IEnumerable<Product> products)
        {
            if (products == null || !products.Any())
            {
                throw new ArgumentException("Product list is null or empty.", nameof(products));
            }

            var firstName = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.GivenName)?.Value ?? "Unknown";
            var lastName = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Surname)?.Value ?? "Unknown";

            try
            {

                foreach (var product in products)
                {
                    var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductName == product.ProductName && p.ProductCategory == product.ProductCategory);

                    if (existingProduct != null)
                    {
                        existingProduct.PricePerProduct = product.PricePerProduct;
                        existingProduct.Quantity += product.Quantity;
                        existingProduct.TotalAmount = existingProduct.Quantity * product.PricePerProduct;
                        existingProduct.LastStockedBy = $"{firstName} {lastName}";
                        existingProduct.LastStockedDate = DateTime.Now;
                    }
                    else
                    {
                        // Add the new product
                        product.FirstStockedBy = $"{firstName} {lastName}"; ;
                        product.LastStockedBy = $"{firstName} {lastName}"; ;
                        product.LastStockedDate = DateTime.Now;
                        product.FirstStockedDate = DateTime.Now;
                        _context.Products.Add(product);
                    }
                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task AddSingleProductAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentException("No products to add.", nameof(product));
            }

            var firstName = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.GivenName)?.Value ?? "Unknown";
            var lastName = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Surname)?.Value ?? "Unknown";


            try
            {
                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductName == product.ProductName && p.ProductCategory == product.ProductCategory);

                if (existingProduct != null)
                {
                    existingProduct.PricePerProduct = product.PricePerProduct;
                    existingProduct.Quantity += product.Quantity;
                    existingProduct.TotalAmount = existingProduct.Quantity * product.PricePerProduct;
                    existingProduct.LastStockedBy = $"{firstName} {lastName}";
                    existingProduct.LastStockedDate = DateTime.Now;
                }
                else
                {
                    // Add the new product
                    product.FirstStockedBy = $"{firstName} {lastName}";
                    product.LastStockedBy = $"{firstName} {lastName}";
                    product.LastStockedDate = DateTime.Now;
                    product.FirstStockedDate = DateTime.Now;
                    _context.Products.Add(product);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<bool> DeleteAsync(Product product, int? quantity = null)
        {
            int qtyToRemove = quantity ?? product.Quantity;

            if (product.Quantity < qtyToRemove)
            {
                return false;
            }

            product.Quantity -= qtyToRemove;
            product.TotalAmount = product.PricePerProduct * product.Quantity;

            // Get the logged-in user's first name and last name from HttpContext
            var firstName = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.GivenName)?.Value ?? "Unknown";
            var lastName = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Surname)?.Value ?? "Unknown";

            product.LastModifiedBy = $"{firstName} {lastName}";
            product.LastModifiedDate = DateTime.Now;

            if (product.Quantity == 0)
            {
                _context.Products.Remove(product);
            }
            else
            {
                _context.Products.Update(product);
            }

            await _context.SaveChangesAsync();
            return true;
        }



        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(ProductCategory category)
        {
            return await _context.Products
                                       .Where(p => p.ProductCategory == category)
                                       .ToListAsync();
        }

        public async Task<Product> GetByIdAsync(string id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<IEnumerable<Product>> GetByNameAsync(string name)
        {
            return await _context.Products
                .Where(p => p.ProductName == name)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
