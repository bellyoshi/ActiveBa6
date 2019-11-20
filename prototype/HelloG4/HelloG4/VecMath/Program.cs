using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace VecMath
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = new AntlrInputStream(Console.In);
            var lexer = new VecMathASTLexer(input);
            var tokenStream = new CommonTokenStream(lexer);
            var p = new VecMathASTParser(tokenStream);
            p.stat();
        }
    }
}
