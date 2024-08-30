using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonShop.Application.DTOs
{
    public class ResponseDTO
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public IEnumerable<ProductDTO>? InvalidProducts { get; set; }

    }
}
