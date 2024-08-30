﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonShop.Application.DTOs
{
    public class RegisterResponse
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
