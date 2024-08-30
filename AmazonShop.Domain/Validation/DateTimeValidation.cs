using System.ComponentModel.DataAnnotations;

namespace AmazonShop.Domain.Validation
{
    public class DateTimeValidation
    {
        public static ValidationResult? ValidateStockedDate(DateTime date, ValidationContext context)
        {
            if (date > DateTime.UtcNow)
            {
                return new ValidationResult("Stocked date cannot be in the future.");
            }
            return ValidationResult.Success;
        }
    }
}
