using ElronAPI.Domain.Extensions;
using ElronAPI.Domain.Helpers;
using System;
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

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void WhitespaceOrEmptyAccountCacheKey(string account)
        {
            Assert.Throws<ArgumentException>(() => CacheKeyHelper.GetAccountCacheKey(account));
        }


        [Fact]
        public void ValidAccountCacheKey()
        {
            string account = "51671327713";
            string expected = $"account_{account}";

            string actual = CacheKeyHelper.GetAccountCacheKey(account);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, "validstring")]
        [InlineData("validstring", null)]
        public void NullTrainTimesCacheKey(string origin, string destination)
        {
            Assert.Throws<ArgumentNullException>(() => CacheKeyHelper.GetTrainTimesCacheKey(origin, destination));
        }

        [Theory]
        [InlineData("", "validstring")]
        [InlineData("validstring", "")]
        [InlineData(" ", "validstring")]
        [InlineData("validstring", " ")]
        public void WhitespaceOrEmptyTrainTimesCacheKey(string origin, string destination)
        {
            Assert.Throws<ArgumentException>(() => CacheKeyHelper.GetTrainTimesCacheKey(origin, destination));
        }

        [Theory]
        [InlineData("riisipere", "tallinn")]
        [InlineData("nõmme", "pääsküla")]
        [InlineData("lilleküla", "kivimäe")]
        [InlineData("järve", "keila rongijaam")]
        public void ValidTrainTimesCacheKey(string origin, string destination)
        {
            string expected = $"traintimes_{origin.RemoveWhiteSpace()}-{destination.RemoveWhiteSpace()}";

            string actual = CacheKeyHelper.GetTrainTimesCacheKey(origin, destination);

            Assert.Equal(expected, actual);
        }
    }
}
