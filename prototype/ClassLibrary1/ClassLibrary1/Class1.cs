using Antlr4.Runtime;

namespace ClassLibrary1
{
    public class Class1
    {
        static public int GetExprVal(string src)
        {
            var inputStream = new AntlrInputStream(src);
            var lexer = new ab6helloLexer(inputStream);
            var commonTokenStream = new CommonTokenStream(lexer);
            var parser = new ab6helloParser(commonTokenStream);
            var tree = parser.expr();
            var exvisitor = new ExVisitor();
            var ex = exvisitor.Visit(tree);
            var e = System.Linq.Expressions.Expression.Lambda<Func<int>>(ex);
            var f = e.Compile();

            return f();
        }
    }
}