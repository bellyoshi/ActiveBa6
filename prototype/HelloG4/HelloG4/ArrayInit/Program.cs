using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace ArrayInit
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = new AntlrInputStream(Console.In);
            // create a lexer that feeds off of input CharStream
            var lexer = new ArrayInitLexer(input);
            // create a buffer of tokens pulled from the lexer
            var tokens = new CommonTokenStream(lexer);
            // create a parser that feeds off the tokens buffer
            var parser = new ArrayInitParser(tokens);
            IParseTree tree = parser.init(); // begin parsing at init rule
            var walker = new ParseTreeWalker();
            walker.Walk(new ShortToUnicodeString(), tree);
            Console.WriteLine();
            Console.ReadKey();
            
        }
    }
}
