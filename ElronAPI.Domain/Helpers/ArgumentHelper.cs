using System;

namespace ElronAPI.Domain.Helpers
{
    public static class ArgumentHelper
    {
        public static void AssertNotNull(string value, string parameter)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameter);
            }
        }

        public static void AssertNotNullOrEmpty(string value, string parameter)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Can't be null or empty", parameter);
            }
        }
    }
}