using HooperMedia.Core.BusinessRules;

namespace HooperMedia.Core.Constants
{
    public static class ValidationErrorMessages
    {
        public static readonly string PersonNameInvalid = $"Person name must not be null, empty, and must not exceed {PersonBusinessRules.NameMaxLength} characters.";
        public static readonly string PersonDateOfBirthInvalid = $"Date of birth must be less than today and person cannot be older than {PersonBusinessRules.MaximumAgeInYears} years.";
    }
}