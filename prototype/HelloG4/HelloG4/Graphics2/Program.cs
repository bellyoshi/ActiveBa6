using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace Graphics2
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = new AntlrInputStream(Console.In);
            var lexer = new GraphicsLexer(input);
            var tokenStream = new CommonTokenStream(lexer);
            var p = new GraphicsParser(tokenStream);
            p.file();
        }
    }
}
