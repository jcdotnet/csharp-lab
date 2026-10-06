using Orders.ModelValidationDemo.Models;
using System.ComponentModel.DataAnnotations;

namespace Orders.ModelValidationDemo.CustomValidators
{
    public class ProductsListValidatorAttribute : ValidationAttribute
    {
        public string DefaultErrorMessage { get; set; } = "Order should have at least one product";

        public ProductsListValidatorAttribute()
        {
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return null;

            List<Product> products = (List<Product>)value;

            if (products.Count == 0)
            {
                // validation error
                return new ValidationResult(DefaultErrorMessage, 
                    new string[] { nameof(validationContext.MemberName) });
            }

            return ValidationResult.Success;
        }
    }
}
