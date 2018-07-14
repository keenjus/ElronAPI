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
    }
}
