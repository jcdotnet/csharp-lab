using System.ComponentModel.DataAnnotations;

namespace Orders.ModelValidationDemo.CustomValidators
{
    public class MinimumDateValidatorAttribute : ValidationAttribute
    {
        public string DefaultErrorMessage { get; set; } = "Order date should be greater than or equal to {0}";
        public DateTime MinimumDate { get; set; }

        public MinimumDateValidatorAttribute(string minimumDateString)
        {
            MinimumDate = Convert.ToDateTime(minimumDateString);
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return null;
            
            DateTime orderDate = (DateTime)value;

            if (orderDate < MinimumDate)
            {
                // validation error
                return new ValidationResult(string.Format(ErrorMessage ?? DefaultErrorMessage, 
                    MinimumDate.ToString("yyyy-MM-dd")), new string[] { nameof(validationContext.MemberName) });
            }
            return ValidationResult.Success; // no error
        }
    }
}