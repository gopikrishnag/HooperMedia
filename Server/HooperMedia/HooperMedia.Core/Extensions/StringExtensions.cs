
namespace HooperMedia.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool IsValidString(this string? value, int maxLength)
        {
            return !string.IsNullOrWhiteSpace(value) && value.Length <= maxLength;
        }

        public static bool IsValidOptionalString(this string? value, int maxLength)
        {
            return string.IsNullOrWhiteSpace(value) || value.Length <= maxLength;
        }
    }
}
