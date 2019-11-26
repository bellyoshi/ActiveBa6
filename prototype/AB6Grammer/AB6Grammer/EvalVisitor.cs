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
        StringBuilder declMethods = new StringBuilder();

        public override string VisitProg([NotNull] AB6Parser.ProgContext context)
        {
            return
                "public class CSHello {\n" +
                declMethods
                +
                    "public static void Main(){ \n"
               + base.VisitProg(context) +
                    "}\n}\n";

        }

        public override string VisitCallSub([NotNull] AB6Parser.CallSubContext context)
        {
            var id = context.ID().GetText();
            return $"{id}();";
        }


        public override string VisitDeclSub([NotNull] AB6Parser.DeclSubContext context)
        {
            var id = context.ID().GetText();
            declMethods.Append($"private static void {id}()\n");
            declMethods.Append("{");
            for(int i  = 0; i < context.linestat().Length; i++)
            {
                declMethods.Append(VisitLinestat(context.linestat(i)));
            }
            declMethods.Append("}");
            return "";
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

        public override string VisitIfstat([NotNull] AB6Parser.IfstatContext context)
        {
            var expr = context.expr().GetText();
            var stat1 = Visit(context.stat(0));
            if (null != context.stat(1)){
                var stat2 = Visit(context.stat(1));
                return $"if (0 < {expr}){{{stat1}}}else{{{stat2}}}";
            }
            else
            {
                return $"if (0 < {expr}){{{stat1}}}";
            }
        }

        public override string VisitIfstatMulti([NotNull] AB6Parser.IfstatMultiContext context)
        {
            var expr = context.expr().GetText();
            var stat1 = Visit(context.linestat(0));
            if (null != context.linestat(1))
            {
                var stat2 = Visit(context.linestat(1));
                return $"if (0 < {expr}){{{stat1}}}else{{{stat2}}}";
            }
            else
            {
                return $"if (0 < {expr}){{{stat1}}}";
            }
        }

        public override string VisitForstat([NotNull] AB6Parser.ForstatContext context)
        {
            var id = context.ID().GetText();
            var expr = context.INT(0).GetText();
            var assign = getAssignCS(id, expr);
            var to = context.INT(1).GetText();

            return $"for({assign}{id}<={to};{id}++)\n" +
            "{\n" +
             Visit(context.linestat(0))
            + "}\n";
        }

        private string getAssignCS(string id , string exprvalue)
        {
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

        public override string VisitAssign([NotNull] AB6Parser.AssignContext context)
        {
            var id = context.ID().GetText();
            var expr = VisitExpr(context.expr());
            return getAssignCS(id , expr);
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
