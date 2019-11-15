using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace AntlrCalc
{
    class Program
    {
        static void Main(string[] args)
        {
            string parsedString = "1024 + 256";
            var inputStream = new AntlrInputStream(parsedString);
            var lexer = new Combined1Lexer(inputStream);
            var commonTokenStream = new CommonTokenStream(lexer);
            var parser = new Combined1Parser(commonTokenStream);
            var graphContext = parser.operation();
            Console.WriteLine(graphContext.ToStringTree());
        }
    }
}
