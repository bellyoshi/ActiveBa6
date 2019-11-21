using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Expr
{
    class Program
    {
        static void Main(string[] args)
        {
            Expr(args);
        }



        static void Expr(string [] args)
        {
            string inputfilepath = null;
            if (0 < args.Length) inputfilepath = args[0];
            var ist = Console.In;
            if (inputfilepath != null) ist = new System.IO.StringReader(inputfilepath);
            var input = new AntlrInputStream(ist);
            var lexer = new ExprLexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new ExprParser(tokens);
            IParseTree tree = parser.prog();
            var eval = new EvalVisitor();
            eval.Visit(tree);
            Console.ReadKey();

        }
    }
}
