using Microsoft.AspNetCore.Mvc.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace Demo.Validators
{
    public class BirthDateAttribute : ValidationAttribute
    {
        public int MinimumAge { get; set; } = 18;
        public string DefaultErrorMessage { get; set; } = "User must be {0} years old or older";

        public BirthDateAttribute() {}
        public BirthDateAttribute(int minimumAge)
        {   
            MinimumAge = minimumAge;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return null;

            DateTime birthDate = (DateTime)value;
            if (birthDate.AddYears(MinimumAge) > DateTime.Now)
            {
                // return new ValidationResult("User must be 18 or older");
                // return new ValidationResult(ErrorMessage);
                return new ValidationResult(string.Format(ErrorMessage ?? DefaultErrorMessage, MinimumAge));
            }

            return ValidationResult.Success;
        }
    }
}
