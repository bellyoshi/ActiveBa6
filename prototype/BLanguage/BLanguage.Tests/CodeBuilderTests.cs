using NUnit.Framework;

namespace BLanguage
{
    public  class CodeBuilderTests
    {
        [Test]
        public void GetEmitLocalsTest1()
        {
            var sut = new CodeBuilder();
            var actual = sut.GetEmitLocals("1", "int");
            Assert.AreEqual(".locals init ( int32 1)", actual);
        }

        [Test]
        public void GetEmitLocalsTest2()
        {
            var sut = new CodeBuilder();
            var actual = sut.GetEmitLocals(
                new string[] { "float64", "float32" },
                new string[] { "1.0", "2.0" }
                );
            Assert.AreEqual(".locals init ( float64 1.0, float32 2.0)" ,actual);
        }
    }
}
