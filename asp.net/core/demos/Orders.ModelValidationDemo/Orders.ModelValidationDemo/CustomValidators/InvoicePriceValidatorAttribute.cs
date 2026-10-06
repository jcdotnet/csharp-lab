using Orders.ModelValidationDemo.Models;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Orders.ModelValidationDemo.CustomValidators
{
    public class InvoicePriceValidatorAttribute : ValidationAttribute
    {
        public string DefaultErrorMessage { get; set; } = "Invoice Price should be equal to the total cost of all products (i.e. {0}) in the order.";

        public InvoicePriceValidatorAttribute()
        {
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return null;

            // get Products property reference, using reflection
            PropertyInfo? OtherProperty = validationContext.ObjectType.GetProperty(nameof(Order.Products));
            if (OtherProperty == null) return null;

            // get value of Products property of the current object of Order class
            List<Product> products = (List<Product>)OtherProperty.GetValue(validationContext.ObjectInstance)!;

            double totalPrice = 0;
            foreach (Product product in products)
            {
                totalPrice += product.Price * product.Quantity;
            }

            double actualPrice = (double)value;

            if (totalPrice > 0)
            {
                if (totalPrice != actualPrice)
                {
                    // model error
                    return new ValidationResult(string.Format(ErrorMessage ?? DefaultErrorMessage, totalPrice), 
                        new string[] { nameof(validationContext.MemberName) });
                }
            }
            else
            {
                // model error is no products found
                return new ValidationResult("No products found to validate invoice price", 
                    new string[] { nameof(validationContext.MemberName) });
            }

            // success
            return ValidationResult.Success;
        }
    }
}
