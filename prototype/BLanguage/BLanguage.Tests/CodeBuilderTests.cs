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

        public void GetEmitLocalsTest2()
        {
            var sut = new CodeBuilder();
            var actual = sut.GetEmitLocals(
                new string[] { "1", "2" }, 
                new string[] {"double", "float");
            Assert.AreEqual(".locals init ( int32 12345)", actual);
        }
    }
}
