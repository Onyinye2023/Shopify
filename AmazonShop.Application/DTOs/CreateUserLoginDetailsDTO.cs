using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonShop.Application.DTOs
{
    public class CreateUserLoginDetailsDTO
    {
        [Required(ErrorMessage = " User name is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = " Password is required")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()><|{}]).{8,}$",
           ErrorMessage = "Password must contain at least one capital letter, " +
           "one small letter, one special character: !@#$%^&*()<>?|], and a minimum of eight characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password required")]
        [Compare("Password", ErrorMessage = "Confirm password must match the password")]
        public string ConfirmPassword { get; set; }

    }
}
