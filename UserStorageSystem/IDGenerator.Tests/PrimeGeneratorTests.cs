namespace IDGenerator.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PrimeGeneratorTests
    {
        private readonly PrimeGenerator generator = new PrimeGenerator();

        [TestMethod]
        public void GetId_ZeroPreviousId_ReturnOne()
        {
            var expected = 1;
            var actual = this.generator.GenerateId(0);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetId_OnePreviousId_ReturnTwo()
        {
            var expected = 2;
            this.generator.GenerateId(0);
            var actual = this.generator.GenerateId(1);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetId_FifteenPreviousId_ReturnSeventeen()
        {
            var expected = 17;
            var actual = this.generator.GenerateId(15);
            Assert.AreEqual(expected, actual);
        }
    }
}
