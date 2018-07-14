using ElronAPI.Domain.Extensions;
using ElronAPI.Domain.Helpers;
using System;
using Xunit;

namespace ElronAPI.Tests
{
    // TODO: better test naming
    public class CacheKeyHelperShould
    {
        public class AccountCacheKey
        {
            [Fact]
            public void ThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => CacheKeyHelper.GetAccountCacheKey(null));
            }

            [Theory]
            [InlineData("")]
            [InlineData(" ")]
            public void ThrowArgumentException_If_EmptyOrWhiteSpace(string account)
            {
                Assert.Throws<ArgumentException>(() => CacheKeyHelper.GetAccountCacheKey(account));
            }


            [Fact]
            public void Return_Valid_CacheKey()
            {
                string account = "51671327713";
                string expected = $"account_{account}";

                string actual = CacheKeyHelper.GetAccountCacheKey(account);

                Assert.Equal(expected, actual);
            }
        }

        public class TrainTimesCacheKey
        {
            [Theory]
            [InlineData(null, "validstring")]
            [InlineData("validstring", null)]
            public void ThrowArgumentNullException(string origin, string destination)
            {
                Assert.Throws<ArgumentNullException>(() => CacheKeyHelper.GetTrainTimesCacheKey(origin, destination));
            }

            [Theory]
            [InlineData("", "validstring")]
            [InlineData("validstring", "")]
            [InlineData(" ", "validstring")]
            [InlineData("validstring", " ")]
            public void ThrowArgumentException_If_EmptyOrWhiteSpace(string origin, string destination)
            {
                Assert.Throws<ArgumentException>(() => CacheKeyHelper.GetTrainTimesCacheKey(origin, destination));
            }

            [Theory]
            [InlineData("riisipere", "tallinn")]
            [InlineData("nõmme", "pääsküla")]
            [InlineData("lilleküla", "kivimäe")]
            [InlineData("järve", "keila rongijaam")]
            public void Return_Valid_CacheKey(string origin, string destination)
            {
                string expected = $"traintimes_{origin.RemoveWhiteSpace()}-{destination.RemoveWhiteSpace()}";

                string actual = CacheKeyHelper.GetTrainTimesCacheKey(origin, destination);

                Assert.Equal(expected, actual);
            }
        }
    }
}
