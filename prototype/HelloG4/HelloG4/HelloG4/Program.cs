using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace HelloG4
{
    class Program
    {
        static void Main(string[] args)
        {
            string parsedString = "hello abc";
            var inputStream = new AntlrInputStream(parsedString);
            var lexer = new HelloLexer(inputStream);
            var commonTokenStream = new CommonTokenStream(lexer);
            var parser = new HelloParser(commonTokenStream);
            var graphContext = parser.r();
            Console.WriteLine(graphContext.ToStringTree());
        }
    }
}
