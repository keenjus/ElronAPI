using System;
using System.Text;
using ElronAPI.Domain.Helpers;
using System.Text.RegularExpressions;

namespace ElronAPI.Domain.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Removes all whitespace from string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveWhiteSpace(this string input)
        {
            ArgumentHelper.AssertNotNull(input, nameof(input));

            return Regex.Replace(input, @"\s+", "");
        }

        public static string LowerCaseFirstLetter(this string input)
        {
            ArgumentHelper.AssertNotNull(input, nameof(input));

            if (input.Length == 0) return input;

            var arr = input.ToCharArray();

            arr[0] = char.ToLowerInvariant(arr[0]);

            return new string(arr);
        }
    }
}
