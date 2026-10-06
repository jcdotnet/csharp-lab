using Demo.Validators;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class User : IValidatableObject
    {
        // [Required] // predefined error message
        // [Required (ErrorMessage = "User name cannot be empty")]
        [Required(ErrorMessage = "{0} cannot be empty")]
        [Display(Name = "User name")]
        [StringLength(
            50, // 1
            MinimumLength = 3, // 2
            ErrorMessage = "{0} must contain at least {2} characters"
        )]
        public string? Name { get; set; } // 0

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        // [ValidateNever] // if we dont want validation on this field
        [Phone]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "{0} cannot be empty")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}",
            ErrorMessage = "{0} must include a minimum of eight characters " +
            "including at least one lowercase letter, one uppercase letter, " +
            "one digit, and one special character."
        )]
        public string? Password { get; set; }

        [Required(ErrorMessage = "{0} cannot be empty")]
        [Compare("Password", ErrorMessage = "{0} and {1} do not match")]
        public string? ConfirmPassword { get; set; }

        // not added to the model // just for learning purposes
        //[Range(18, 70, ErrorMessage = "{0} must be between {1} and {2} ")]
        //public int Age { get; set; }

        // not added to the model // just for learning purposes
        //[Url]
        //public string Website { get; set; }

        [BindNever]
        public string AnotherProperty {  get; set; }

        [BirthDate(18, ErrorMessage = "User must be {0} or older" )]
        public DateTime? BirthDate { get; set; }

        public int? Age { get; set; }

        public DateTime FromDate { get; set; }
        [DateRange("FromDate", ErrorMessage ="{1} must be older than {0}")] 
        public DateTime ToDate { get; set; }

        public override string? ToString()
        {
            return $"{Name}. Email: {Email}. Phone: {Phone}";
        }

        // IValidatableObject (in case we want to create custom validation specific to this class)
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Age.HasValue == false && BirthDate.HasValue == false)
                yield return new ValidationResult("Either user age of birthdate must be supplied");
        }
    }
}
