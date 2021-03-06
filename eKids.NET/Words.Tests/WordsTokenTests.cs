using NUnit.Framework;
using eKids;
using System.Linq;

namespace Words.Tests
{
    public class WordsTokenTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestNotNull()
        {
            // AAA = Arrange, Act, Assert
            var input = "Hello, World!";

            var actual = input.FindTokens();

            Assert.NotNull(actual);
        }

        [Test]
        public void TestNonZeroReturn()
        {
            // AAA = Arrange, Act, Assert
            var input = "Hello, World!";

            var actual = input.FindTokens();

            Assert.True(actual.Count() > 0);
        }

        [Test]
        public void TestNumberOfTokensCorrect()
        {
            // AAA = Arrange, Act, Assert
            var input = "Hello   ,   World!";

            var actual = input.FindTokens().Count();
            var expected = 6;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestAllTokens()
        {
            // AAA = Arrange, Act, Assert
            var input = "Hello   ,   World!";
            var expected = new string[] { "Hello", "   ", ",", "   ", "World", "!" };

            var actual = input.FindTokens().ToList();

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }
    }
}