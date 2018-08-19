using System;
using System.Collections.Generic;
using System.Text;
using ElronAPI.Domain.Extensions;
using Xunit;

namespace ElronAPI.Tests
{
    public class StringExtensionsShould
    {
        public class RemoveWhiteSpace
        {
            [Fact]
            public void Return_Correct_String()
            {
                const string expected = "testwooow";

                string actual = "     test     wooow     ".RemoveWhiteSpace();

                Assert.Equal(expected, actual);
            }

            [Fact]
            public void ThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => { (null as string).RemoveWhiteSpace(); });
            }
        }

        public class LowerCaseFirstLetter
        {
            [Fact]
            public void Return_Correct_String()
            {
                const string expected = "testString";

                string actual = "TestString".LowerCaseFirstLetter();

                Assert.Equal(expected, actual);
            }

            [Fact]
            public void ThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => { (null as string).LowerCaseFirstLetter(); });
            }
        }
    }
}
