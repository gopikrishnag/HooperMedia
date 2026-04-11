namespace HooperMedia.Core.BusinessRules
{
    public static class PersonBusinessRules
    {
        public const int NameMaxLength = 50;
        public const int MaximumAgeInYears = 100;
        public static DateTime Today => DateTime.UtcNow.Date;
    }
}