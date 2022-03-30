using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Language.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual("2\r\n", Program.ParseAndRun("println 1+1") );
            Assert.AreEqual("6\r\n", Program.ParseAndRun("println 2*3"));
            Assert.AreEqual("5\r\n", Program.ParseAndRun("println 10/2"));
            Assert.AreEqual("3\r\n", Program.ParseAndRun("println 4-1"));
        }

        [TestMethod]
        public void VariantTest()
        {
            var text =
                @"
                    A = 1;
                    B = 2;
                    C = A + B;
                    println C;
                ";
            Assert.AreEqual("3\r\n", Program.ParseAndRun(text));
        }
        [TestMethod]
        public void ManyVariantTest()
        {
            var text = @"
                    A=1;
                    B=2;
                    C=3;
                    D=4;
                    E=5;
                    F=6;
                    G=7
                    H=8;
                    I=9;
                    println I;
                ";
            Assert.AreEqual("9\r\n",Program.ParseAndRun(text));
        }
    }
}