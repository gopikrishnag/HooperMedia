using HooperMedia.Core.BusinessRules;

namespace HooperMedia.Core.Extensions
{

    public static class DateTimeExtensions
    {

        public static bool IsValidDateOfBirth(this DateTime dateOfBirth)
        {
            var today = PersonBusinessRules.Today;

            if (dateOfBirth.Date >= today)
            {
                return false;
            }

            // Calculate age and ensure it does not exceed PersonBusinessRules.MaximumAgeInYears
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age))
            {
                age--;
            }

            return age <= PersonBusinessRules.MaximumAgeInYears && age >= 0;
        }

    }
}