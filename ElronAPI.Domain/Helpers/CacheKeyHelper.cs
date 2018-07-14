using System;

namespace ElronAPI.Domain.Helpers
{
    public class CacheKeyHelper
    {
        public static string GetAccountCacheKey(string account)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            if (string.IsNullOrWhiteSpace(account))
            {
                throw new ArgumentException("Can't be null or empty", nameof(account));
            }

            return $"account_{account}";
        }
    }
}
