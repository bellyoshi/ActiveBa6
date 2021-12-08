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
            while (true)
            {
                
                var inputStream = new AntlrInputStream(Console.In);
                var lexer = new HelloLexer(inputStream);
                var commonTokenStream = new CommonTokenStream(lexer);
                var parser = new HelloParser(commonTokenStream);
                var tree = parser.prog();
                var exvisitor = new ExVisitor();
                var ex = exvisitor.Visit(tree);
                var e = System.Linq.Expressions.Expression.Lambda<Func<int>>(ex);
                var f = e.Compile();

                Console.WriteLine(tree.ToStringTree());
                f();

            }
        }
    }
}
