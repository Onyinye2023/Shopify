using AmazonShop.Application.Abstraction.IServices.IProductServices;
using AmazonShop.Application.DTOs;
using AmazonShop.Domain.Enums;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmazonShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class ProductController : ControllerBase
    {
        private readonly IProductServices _productService;
        private readonly ILogger<ProductController> _logger;
        private readonly IMapper _mapper;

        public ProductController(ILogger<ProductController> logger, IProductServices productService, IMapper mapper)
        {
            _productService = productService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("stock_singleProduct")]
        public async Task<IActionResult> AddSingleProduct([FromBody] ProductDTO productdto)
        {
            try
            {
                if (productdto == null)
                {
                    return BadRequest("Product data is missing");
                }

                var response = await _productService.AddSingleProductAsync(productdto);

                if (!response.Success)
                {
                    return BadRequest(new { Message = response.Message });
                }

                return Ok("Product added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding product: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("stock_multipleProduct")]
        public async Task<IActionResult> AddMultipleProduct([FromBody] IEnumerable<ProductDTO> productdtos)
        {
            if (productdtos == null || !productdtos.Any())
            {
                _logger.LogError("Product data is missing or empty.");
                return BadRequest("Product data is missing or empty.");
            }

            try
            {
                var response = await _productService.AddMultipleProductAsync(productdtos);

                if (!response.Success)
                {
                    if (response.InvalidProducts != null && response.InvalidProducts.Any())
                    {
                        var invalidProductNames = response.InvalidProducts.Select(p => p.ProductName);
                        return BadRequest(new
                        {
                            Message = response.Message,
                            InvalidProducts = invalidProductNames
                        });
                    }
                    return BadRequest(response.Message);
                }

                if (response.InvalidProducts != null && response.InvalidProducts.Any())
                {
                    var invalidProductNames = response.InvalidProducts.Select(p => p.ProductName);
                    return Ok(new
                    {
                        Message = response.Message,
                        InvalidProducts = invalidProductNames
                    });
                }

                return Ok(response.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding products: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }



        [HttpGet("get_allProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                if (products == null)
                {
                    return NotFound("No Product found");
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting all products: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }



        [HttpGet("get_productsByCategory/{category}")]
        public async Task<IActionResult> GetProductsByCategory(ProductCategory category)
        {
            try
            {
                var products = await _productService.GetProductByCategory(category);
                if (products == null)
                {
                    return NotFound($"No Product found with the category {category}");
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting products by category: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }



        [HttpGet("get_productsByName/{name}")]
        public async Task<IActionResult> GetProductsByName(string name)
        {
            try
            {
                var products = await _productService.GetProductByName(name);
                if (products == null)
                {
                    return NotFound($"No Product found with the name {name}");
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting products by name: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpDelete("delete_product")]
        public async Task<IActionResult> DeleteProduct(string id, int quantity)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id, quantity);
                if (!result.Success)
                {
                    return BadRequest(result.Message);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting product by ID: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}

