using NUnit.Framework;

namespace YK.Socket.Extensions.Tests
{
    public class StringExtensionsTests
    {
        [Test()]
        [TestCase("414243")]
        [TestCase("41 42 43")]
        public void HexToByteArrayBaseTest(string hex)
        {
            var converted = ArrayHelpers.HexToByteArray(hex);
            Assert.That(converted, Is.EqualTo("ABC"u8.ToArray()));
        }

        [Test]
        public void HexTobyteArrayInvalidLengthTest()
        {
            var converted = ArrayHelpers.HexToByteArray("4");
            Assert.That(converted, Is.Empty);
        }
    }
}