using System.Text.RegularExpressions;
using ElronAPI.Domain.Extensions;

namespace ElronAPI.Domain.Helpers
{
    public static class CacheKeyHelper
    {
        public static string GetAccountCacheKey(string account)
        {
            ArgumentHelper.AssertNotNull(account, nameof(account));
            ArgumentHelper.AssertNotNullOrEmpty(account, nameof(account));

            return $"account_{account}";
        }

        public static string GetTrainTimesCacheKey(string origin, string destination)
        {
            ArgumentHelper.AssertNotNull(origin, nameof(origin));
            ArgumentHelper.AssertNotNullOrEmpty(origin, nameof(origin));

            ArgumentHelper.AssertNotNull(destination, nameof(destination));
            ArgumentHelper.AssertNotNullOrEmpty(destination, nameof(destination));

            return $"traintimes_{origin.RemoveWhiteSpace()}-{destination.RemoveWhiteSpace()}";
        }
    }
}
