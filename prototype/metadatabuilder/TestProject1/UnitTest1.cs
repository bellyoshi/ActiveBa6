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
            var result = ConsoleApp1.Class1.BuildAndRun("ConsoleApplication.exe");
            Assert.AreEqual("Hello MSIL\r\n", result);
        }
    }
}