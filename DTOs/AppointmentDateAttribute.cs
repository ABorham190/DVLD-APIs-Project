using System.ComponentModel.DataAnnotations;

namespace dvld_api.models
{
    public class AppointmentDateAttribute:ValidationAttribute
    {
        private readonly int _maximumWaitingPeriod;

        public AppointmentDateAttribute(int maximumWaitingPeriod)
        {
            _maximumWaitingPeriod = maximumWaitingPeriod;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            DateTime AppointmentDate;
            bool isvalid = DateTime.TryParse(value?.ToString(), out AppointmentDate);

            if (!isvalid)
            {
                return new ValidationResult("Invalid Date Format");
            }

            if (AppointmentDate < DateTime.Now)
            {
                return new ValidationResult("AppointmentDate must be in the future");
            }

            TimeSpan Difference = AppointmentDate-DateTime.Now;

            var WaitingPeriod = Difference.Days;

            if (WaitingPeriod > _maximumWaitingPeriod)
            {
                return new ValidationResult($"Waiting Period must not exceed {_maximumWaitingPeriod} days");
            }

            return ValidationResult.Success;

        }
    }
}
