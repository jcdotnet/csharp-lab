using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Demo.Validators
{
    public class DateRangeAttribute : ValidationAttribute
    {
        public string OtherPropertyName { get; set; }
        public DateRangeAttribute(string otherPropertyName)
        {
            OtherPropertyName = otherPropertyName;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return null;

            PropertyInfo? propertyInfo = validationContext.ObjectType.GetProperty(OtherPropertyName);

            if (propertyInfo == null) return null;

            DateTime toDate = (DateTime)value;
            var fromDate = (DateTime)propertyInfo.GetValue(validationContext.ObjectInstance);


            if (fromDate > toDate)  
                return new ValidationResult(string.Format(
                    ErrorMessage, 
                    OtherPropertyName, 
                    validationContext.MemberName
                ));
            return ValidationResult.Success;
        }
    }
}
