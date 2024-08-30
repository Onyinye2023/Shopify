using System.ComponentModel.DataAnnotations;

namespace AmazonShop.Application.DTOs
{
    public class AdminRegisterDTO
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = " Password is required")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()><|{}]).{8,}$",
            ErrorMessage = "Password must contain at least one capital letter, " +
            "one small letter, one special character: !@#$%^&*()<>?|], and a minimum of eight characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password required")]
        [Compare("Password", ErrorMessage = "Confirm password must match the password")]
        public string ConfirmPassword { get; set; } = string.Empty;
        [Required]
        public int RoleId { get; set; } = 0;

    }
}
