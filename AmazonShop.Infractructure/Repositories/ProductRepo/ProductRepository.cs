using AmazonShop.Application.Abstraction.IRepositories.IProductRepo;
using AmazonShop.Domain.Enums;
using AmazonShop.Domain.Models;
using AmazonShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace AmazonShop.Infractructure.Repositories.ProductRepo
{
    public class ProductRepository : IProductRepository<Product>
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddMultipleProductAsync(IEnumerable<Product> products, string username)
        {
            if (products == null || !products.Any())
            {
                throw new ArgumentException("Product list is null or empty.", nameof(products));
            }

            var getUser = await _context.Users
               .Include(r => r.Role)
               .FirstOrDefaultAsync(u => u.UserName == username);

            if (getUser == null)
            {
                throw new ArgumentException("User not found.", nameof(username));
            }

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
                        existingProduct.LastStockedBy = getUser.FirstName + " " + getUser.LastName;
                        existingProduct.LastStockedDate = DateTime.Now;
                    }
                    else
                    {
                        // Add the new product
                        product.FirstStockedBy = getUser.FirstName + " " + getUser.LastName;
                        product.LastStockedBy = getUser.FirstName + " " + getUser.LastName;
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

        public async Task AddSingleProductAsync(Product product, string username)
        {
            if (product == null)
            {
                throw new ArgumentException("No products to add.", nameof(product));
            }

            var getUser = await _context.Users
                .Include(r => r.Role)
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (getUser == null)
            {
                throw new ArgumentException("User not found.", nameof(username));
            }

            try
            {
                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductName == product.ProductName && p.ProductCategory == product.ProductCategory);

                if (existingProduct != null)
                {
                    existingProduct.PricePerProduct = product.PricePerProduct;
                    existingProduct.Quantity += product.Quantity;
                    existingProduct.TotalAmount = existingProduct.Quantity * product.PricePerProduct;
                    existingProduct.LastStockedBy = getUser.FirstName + " " + getUser.LastName;
                    existingProduct.LastStockedDate = DateTime.Now;
                }
                else
                {
                    // Add the new product
                    product.FirstStockedBy = getUser.FirstName + " " + getUser.LastName;
                    product.LastStockedBy = getUser.FirstName + " " + getUser.LastName;
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


        public async Task<bool> DeleteAsync(Product product, int quantity)
        {
            if (product.Quantity < quantity)
            {
                return false; // Not enough quantity to delete
            }

            product.Quantity -= quantity;

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
