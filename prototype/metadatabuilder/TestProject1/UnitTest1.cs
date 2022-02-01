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
            var result = new ConsoleApp1.Class1("ConsoleApplication.exe").BuildAndRun();
            Assert.AreEqual("Hello MSIL\r\n", result);
        }
    }
}