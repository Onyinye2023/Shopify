using AmazonShop.Application.Abstraction.IRepositories.IProductRepo;
using AmazonShop.Application.Abstraction.IServices.IProductServices;
using AmazonShop.Application.DTOs;
using AmazonShop.Domain.Enums;
using AmazonShop.Domain.Models;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace AmazonShop.Application.Services.ProductServices
{
    public class ProductServices : IProductServices
    {
        private readonly IProductRepository<Product> _productRepo;
        private readonly ILogger<ProductServices> _logger;
        private readonly IMapper _mapper;

        public ProductServices(IProductRepository<Product> productRepo, ILogger<ProductServices> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _productRepo = productRepo;

        }
        public async Task<ResponseDTO> AddMultipleProductAsync(IEnumerable<ProductDTO> productdtos)
        {
            if (productdtos == null || !productdtos.Any())
            {
                _logger.LogError("ProductDTO is null or empty.");
                return new ResponseDTO { Success = false, Message = "Invalid product data." };
            }

            var invalidProducts = new List<ProductDTO>();
            var validProducts = new List<Product>();

            foreach (var productdto in productdtos)
            {
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(productdto);

                bool isValid = Validator.TryValidateObject(productdto, validationContext, validationResults, true);

                if (!isValid)
                {
                    _logger.LogError($"Validation failed for product: {productdto.ProductName}. Errors: {string.Join(", ", validationResults.Select(r => r.ErrorMessage))}");
                    invalidProducts.Add(productdto);
                    continue;
                }

                if (!Enum.IsDefined(typeof(ProductCategory), productdto.ProductCategory))
                {
                    _logger.LogError($"Invalid category value for product: {productdto.ProductName}");
                    invalidProducts.Add(productdto);
                    continue;
                }

                var product = new Product
                {
                    ProductName = productdto.ProductName,
                    ProductCategory = productdto.ProductCategory,
                    ProductCategoryName = productdto.ProductCategoryName,
                    Quantity = productdto.Quantity,
                    PricePerProduct = productdto.PricePerProduct,
                    TotalAmount = productdto.Quantity * productdto.PricePerProduct,
                    FirstStockedDate = DateTime.Now
                };

                validProducts.Add(product);
            }

            if (!validProducts.Any())
            {
                return new ResponseDTO { Success = false, Message = "No valid products to add.", InvalidProducts = invalidProducts };
            }

            try
            {
                await _productRepo.AddMultipleProductAsync(validProducts);
                return new ResponseDTO { Success = true, Message = "Products added successfully.", InvalidProducts = invalidProducts };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the products.");
                return new ResponseDTO { Success = false, Message = "An error occurred while adding the products.", InvalidProducts = invalidProducts };
            }
        }

        public async Task<ResponseDTO> AddSingleProductAsync(ProductDTO productdto)
        {
            if (productdto == null)
            {
                _logger.LogError("ProductDTO is null.");
                return new ResponseDTO { Success = false, Message = "Invalid product data." };
            }


            var quantityCheck = Validator.TryValidateProperty
              (
                productdto.Quantity,
                    new ValidationContext(productdto)
                    {
                        MemberName = nameof(productdto.Quantity)
                    }, null

              );

            var pricePerProductCheck = Validator.TryValidateProperty
               (
                  productdto.PricePerProduct,
                        new ValidationContext(productdto)
                        {
                            MemberName = nameof(productdto.PricePerProduct)
                        }, null

               );


            if (!quantityCheck || !pricePerProductCheck)
            {
                return new ResponseDTO { Success = false, Message = "Failed to Validate Date OR Quantity OR Price" };
            }


            if (!Enum.IsDefined(typeof(ProductCategory), productdto.ProductCategory))
            {
                return new ResponseDTO { Success = false, Message = "Invalid category value" };
            }

            try
            {
                //var product = _mapper.Map<Product>(productdto);
                var product = new Product
                {
                    ProductName = productdto.ProductName,
                    ProductCategory = productdto.ProductCategory,
                    ProductCategoryName = productdto.ProductCategoryName,
                    Quantity = productdto.Quantity,
                    PricePerProduct = productdto.PricePerProduct,
                    TotalAmount = productdto.Quantity * productdto.PricePerProduct,
                    FirstStockedDate = DateTime.Now

                };
                if (product == null)
                {
                    _logger.LogError("Mapping of ProductDTO to Product failed.");
                    return new ResponseDTO { Success = false, Message = "Mapping error." };
                }

                await _productRepo.AddSingleProductAsync(product);
                return new ResponseDTO { Success = true, Message = "Product added successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the product.");
                return new ResponseDTO { Success = false, Message = "An error occurred while adding the product." };
            }
        }


        public async Task<ResponseDTO> DeleteProductAsync(string id, int quantity)
        {
            try
            {
                var product = await _productRepo.GetByIdAsync(id);
                if (product == null)
                {
                    _logger.LogWarning($"Product with ID {id} not found");
                    return new ResponseDTO { Success = false, Message = $"Product with ID {id} not found" };
                }

                if (quantity > product.Quantity)
                {
                    _logger.LogWarning($"Insufficient quantity for product ID {id}. Requested: {quantity}, Available: {product.Quantity}");
                    return new ResponseDTO { Success = false, Message = $"Insufficient quantity for product ID {id}. Requested: {quantity}, Available: {product.Quantity}" };
                }

                await _productRepo.DeleteAsync(product, quantity);
                return new ResponseDTO { Success = true, Message = "Product deleted successfully" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ResponseDTO { Success = false, Message = "Internal server error" };
            }
        }

        public async Task<IEnumerable<GetProductDTO>> GetAllProductsAsync()
        {
            try
            {
                var projects = await _productRepo.GetAllAsync();
                return _mapper.Map<IEnumerable<GetProductDTO>>(projects);

            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<GetProductDTO>> GetProductByCategory(ProductCategory category)
        {
            try
            {
                var product = await _productRepo.GetByCategoryAsync(category);
                if (product == null)
                {
                    _logger.LogWarning($"Product with the category {category} not found");
                    return null;
                }

                return _mapper.Map<IEnumerable<GetProductDTO>>(product);


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<GetProductDTO>> GetProductByName(string name)
        {
            try
            {
                var products = await _productRepo.GetByNameAsync(name);
                if (products == null || !products.Any())
                {
                    _logger.LogWarning($"No products found with the name {name}");
                    return Enumerable.Empty<GetProductDTO>();
                }

                return _mapper.Map<IEnumerable<GetProductDTO>>(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving products by the name {name}");
                return Enumerable.Empty<GetProductDTO>();
            }
        }

    }
}
