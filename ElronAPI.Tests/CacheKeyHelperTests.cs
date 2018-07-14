using System;
using ElronAPI.Domain.Helpers;
using Xunit;

namespace ElronAPI.Tests
{
    // TODO: better test naming
    public class CacheKeyHelperTests
    {
        [Fact]
        public void NullAccountCacheKey()
        {
            Assert.Throws<ArgumentNullException>(() => CacheKeyHelper.GetAccountCacheKey(null));
        }

        [Fact]
        public void EmptyAccountCacheKey()
        {
            Assert.Throws<ArgumentException>(() => CacheKeyHelper.GetAccountCacheKey(""));
        }

        [Fact]
        public void WhitespaceAccountCacheKey()
        {
            Assert.Throws<ArgumentException>(() => CacheKeyHelper.GetAccountCacheKey(" "));
        }

        [Fact]
        public void CorrectAccountCacheKey()
        {
            string account = "51671327713";
            string expected = $"account_{account}";

            string actual = CacheKeyHelper.GetAccountCacheKey(account);

            Assert.Equal(expected, actual);
        }
    }
}
