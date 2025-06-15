using System;
using TestABP.Utils;

namespace TestABP.Validation
{
    public static class ValidationMessage
    {
        public static string FreeStyle(string propertyName, string message)
        {
            return $"{propertyName.ToKebabCase()} {message}";
        }

        public static string Required(string propertyName)
        {
            return FreeStyle(propertyName, "is required");
        }

        public static string IsNotNullOrEmpty(string propertyName)
        {
            return FreeStyle(propertyName, "cannot be null or empty");
        }

        public static string IsNotNullOrWhiteSpace(string propertyName)
        {
            return FreeStyle(propertyName, "cannot be null or whitespace");
        }

        public static string Invalid(string propertyName)
        {
            return FreeStyle(propertyName, "is invalid");
        }

        public static string OutOfRange(string propertyName)
        {
            return FreeStyle(propertyName, "Invalid length");
        }

        public static string MaximumLength(string propertyName, int maxLength)
        {
            return FreeStyle(propertyName, $"cannot be over {maxLength} characters");
        }

        public static string ValueApiEndpointPathRequired(string propertyName)
        {
            return FreeStyle(propertyName, "is required in the API endpoint path");
        }

        public static string FormatInvalid(string propertyName)
        {
            return $"Invalid {propertyName.ToKebabCase()} format";
        }

        public static string MustBeNumberInRange(string propertyName, int start, int end)
        {
            return FreeStyle(propertyName, $"must be between {start} and {end} in length");
        }

        public static string MustBeGreaterThanOrEqualTo(string propertyName, int value)
        {
            return FreeStyle(propertyName, $"must be greater than or equal to '{value}'");
        }

        public static string MustBeNumber(string propertyName)
        {
            return FreeStyle(propertyName, $"value must be numbers only");
        }

        public static string MinimumLength(string propertyName, int minLength)
        {
            return FreeStyle(propertyName, $"minimum length is {minLength} characters");
        }
    }
}
