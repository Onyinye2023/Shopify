using AmazonShop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonShop.Application.DTOs
{
    public class GetProductDTO
    {
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public string? ProductCategoryName { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerProduct { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime FirstStockedDate { get; set; }
        public DateTime LastStockedDate { get; set; }
    }
}
