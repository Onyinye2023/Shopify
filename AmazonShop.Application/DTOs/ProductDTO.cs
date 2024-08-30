using AmazonShop.Domain.Enums;
using AmazonShop.Domain.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonShop.Application.DTOs
{
    public class ProductDTO
    {
        [Required]
        public string ProductName { get; set; }
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

        //[Required]
        // public decimal TotalAmount { get; set; }    

        //[Required]
        //[DataType(DataType.DateTime)]
        //[CustomValidation(typeof(DateTimeValidation), nameof(DateTimeValidation.ValidateStockedDate))]
        //public DateTime FirstStockedDate { get; set; }

        //[Required]
        //[DataType(DataType.Date)]
        //[CustomValidation(typeof(DateTimeValidation), nameof(DateTimeValidation.ValidateStockedDate))]
        //public DateTime LastStockedDate { get; set; }

    }
}
