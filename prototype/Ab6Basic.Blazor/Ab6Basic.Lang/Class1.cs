using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace HelloG4
{
    public class Class1
    {
        static public int GetExprVal(string src)
        {
            var inputStream = new AntlrInputStream(src);
            var lexer = new HelloLexer(inputStream);
            var commonTokenStream = new CommonTokenStream(lexer);
            var parser = new HelloParser(commonTokenStream);
            var tree = parser.expr();
            var exvisitor = new ExVisitor();
            var ex = exvisitor.Visit(tree);
            var e = System.Linq.Expressions.Expression.Lambda<Func<int>>(ex);
            var f = e.Compile();

            return f();
        }
    }
}
