using AmazonShop.Domain.Enums;
using AmazonShop.Domain.Validation;
using System.ComponentModel.DataAnnotations;

namespace AmazonShop.Domain.Models
{
    public class Product
    {
        [Required]
        public string? ProductId { get; set; }

        [Required]
        public string? ProductName { get; set; }

        [Required]
        public ProductCategory ProductCategory { get; set; }
        [Required]
        public string ProductCategoryName { get; set; }

        [Required]
        //[Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price per product must be greater than 0.")]
        public decimal PricePerProduct { get; set; }
        public decimal TotalAmount { get; set; }

        [Required]
        public string FirstStockedBy { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [CustomValidation(typeof(DateTimeValidation), nameof(DateTimeValidation.ValidateStockedDate))]
        public DateTime FirstStockedDate { get; set; }

        [Required]
        public string LastStockedBy { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [CustomValidation(typeof(DateTimeValidation), nameof(DateTimeValidation.ValidateStockedDate))]
        public DateTime LastStockedDate { get; set; }

        public string? LastModifiedBy { get; set; }

        [DataType(DataType.DateTime)]
        [CustomValidation(typeof(DateTimeValidation), nameof(DateTimeValidation.ValidateStockedDate))]
        public DateTime? LastModifiedDate { get; set; }

        public Product()
        {
            ProductId = Guid.NewGuid().ToString();

        }

    }
}
