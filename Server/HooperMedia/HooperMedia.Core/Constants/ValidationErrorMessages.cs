using HooperMedia.Core.BusinessRules;

namespace HooperMedia.Core.Constants
{
    public static class ValidationErrorMessages
    {
        public static readonly string PersonNameInvalid = $"Person name must not be null, empty, and must not exceed {PersonBusinessRules.NameMaxLength} characters.";
        public static readonly string PersonDateOfBirthInvalid = $"Date of birth must be less than today and person cannot be older than {PersonBusinessRules.MaximumAgeInYears} years.";
        public static readonly string AddressLine1Invalid = $"Address Line 1 must not be null, empty, and must not exceed {AddressBusinessRules.AddressLineMaxLength} characters.";
        public static readonly string AddressLine2Invalid = $"Address Line 2 must not exceed {AddressBusinessRules.AddressLineMaxLength} characters.";
        public static readonly string TownOrCityInvalid = $"Town or City must not be null, empty, and must not exceed {AddressBusinessRules.AddressLineMaxLength} characters.";
        public static readonly string ZipOrPostCodeInvalid = $"Zip or Post Code must not be null, empty, and must not exceed {AddressBusinessRules.ZipOrPostCodeMaxLength} characters.";
        public static readonly string CountryInvalid = $"Country must not be null, empty, and must not exceed {AddressBusinessRules.CountryMaxLength} characters.";
    }
}