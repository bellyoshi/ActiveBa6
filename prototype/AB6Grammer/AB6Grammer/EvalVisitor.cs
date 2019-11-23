using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace AB6Grammer
{
    class EvalVisitor : AB6BaseVisitor<string>
    {
        List<String> memory = new List<string>();

        public override string VisitProg([NotNull] AB6Parser.ProgContext context)
        {
            return
                "public class CSHello {\n" +
                    "public static void Main(){ \n"
               + base.VisitProg(context) +
                    "}\n}\n";

        }

        public override string VisitChildren([NotNull] IRuleNode node)
        {
            string s = string.Empty;
            for(var i = 0; i < node.ChildCount; i++)
            {
                s += Visit(node.GetChild(i));
            }
            return s; 
        }
        public override string VisitLinestat([NotNull] AB6Parser.LinestatContext context)
        {
            return VisitStat(context.stat());
        }

        public override string VisitAssign([NotNull] AB6Parser.AssignContext context)
        {
            var id = context.ID().GetText();
            var exprvalue = VisitExpr(context.expr());
            if (!memory.Contains(id))
            {
                memory.Add(id);
                return $"var {id} = {exprvalue};\n";
            }
            else
            {
                return $"{id} = {exprvalue};\n";
            }
        }

        public override string VisitPrint([NotNull] AB6Parser.PrintContext context)
        {
            return "System.Console.WriteLine(" 
            +VisitExpr(context.expr()) +
            ");\n"; 
        }

        public override string VisitExpr([NotNull] AB6Parser.ExprContext context)
        {
            return context.GetText();
        }
    }
}
