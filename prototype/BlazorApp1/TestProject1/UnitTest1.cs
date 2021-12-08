using NUnit.Framework;

namespace TestProject1
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.AreEqual(2, HelloG4.Class1.GetExprVal("1+1"));
            Assert.AreEqual(14, HelloG4.Class1.GetExprVal("2+4*3"));
        }
    }
}