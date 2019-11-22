using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;

namespace AB6Grammer
{
    class EvalVisitor : AB6BaseVisitor<string>
    {
        
        public override string VisitProg([NotNull] AB6Parser.ProgContext context)
        {
             return  
             @"public class CSHello {
                     public static void Main(){ "
                + base.VisitProg(context) +
                     @"}
             }";
        }

        public override string VisitLinestat([NotNull] AB6Parser.LinestatContext context)
        {
            return VisitStat(context.stat());
        }

        public override string VisitStat([NotNull] AB6Parser.StatContext context)
        {
            return base.VisitStat(context);
        }


        public override string VisitPrint([NotNull] AB6Parser.PrintContext context)
        {
            return "System.Console.WriteLine(" 
            +VisitExpr(context.expr()) +
            ");"; 
        }

        public override string VisitExpr([NotNull] AB6Parser.ExprContext context)
        {
            return context.GetText();
        }
    }
}
