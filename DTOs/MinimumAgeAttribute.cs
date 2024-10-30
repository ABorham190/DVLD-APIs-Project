using System.ComponentModel.DataAnnotations;

namespace dvld_api.models
{
    public class MinimumAgeAttribute:ValidationAttribute
    {
        private readonly int _minimumAge;

        public MinimumAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
        }

        protected override ValidationResult IsValid(object value,ValidationContext validationContext)
        {
            DateTime DateOfBirh;
            bool isvalid = DateTime.TryParse(value.ToString(), out DateOfBirh);

            if (!isvalid)
            {
                return new ValidationResult("Invalid Date Format");
            }

            var age=DateTime.Now.Year-DateOfBirh.Year;

            if (DateOfBirh > DateTime.Now.AddYears(-age))
            {
                age--;
            }

            if (age < _minimumAge)
            {
                return new ValidationResult($"Age must be at least {_minimumAge}");
            }

            return ValidationResult.Success;
        }
    }
}
