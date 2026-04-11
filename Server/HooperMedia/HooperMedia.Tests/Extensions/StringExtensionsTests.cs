using HooperMedia.Core.Extensions;

namespace HooperMedia.Tests.Extensions
{
    public class StringExtensionsTests
    {
        [Fact]
        public void IsValidString_WithValidString_ReturnsTrue()
        {
            var validString = "Test String";

            var result = validString.IsValidString(20);

            Assert.True(result);
        }

        [Fact]
        public void IsValidString_WithEmptyString_ReturnsFalse()
        {
            var emptyString = "";

            var result = emptyString.IsValidString(20);

            Assert.False(result);
        }

        [Fact]
        public void IsValidString_WithStringExceedingMaxLength_ReturnsFalse()
        {
            var longString = new string('A', 21);

            var result = longString.IsValidString(20);

            Assert.False(result);
        }

        [Fact]
        public void IsValidString_WithNullString_ReturnsFalse()
        {
            string? nullString = null;

            var result = nullString.IsValidString(20);

            Assert.False(result);
        }

        [Fact]
        public void IsValidOptionalString_WithValidString_ReturnsTrue()
        {
            var validString = "Test String";

            var result = validString.IsValidOptionalString(20);

            Assert.True(result);
        }

        [Fact]
        public void IsValidOptionalString_WithNullString_ReturnsTrue()
        {
            string? nullString = null;

            var result = nullString.IsValidOptionalString(20);

            Assert.True(result);
        }

        [Fact]
        public void IsValidOptionalString_WithEmptyString_ReturnsTrue()
        {
            var emptyString = "";

            var result = emptyString.IsValidOptionalString(20);

            Assert.True(result);
        }

        [Fact]
        public void IsValidOptionalString_WithStringExceedingMaxLength_ReturnsFalse()
        {
            var longString = new string('A', 21);

            var result = longString.IsValidOptionalString(20);

            Assert.False(result);
        }
    }
}
 