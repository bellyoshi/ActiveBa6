using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using System.Diagnostics;

namespace Expr
{
    class EvalVisitor : ExprBaseVisitor<int>
    {
        Dictionary<string, int> memory = new Dictionary<string, int>();

        public override int VisitAssign([NotNull] ExprParser.AssignContext context)
        {
            string id = context.ID().GetText();
            int value = Visit(context.expr());
            memory[id] = value;
            return value;
        }

        public override int VisitPrintExpr([NotNull] ExprParser.PrintExprContext context)
        {
            int value = Visit(context.expr());
            Console.WriteLine(value);
            return 0;
        }

        public override int VisitInt([NotNull] ExprParser.IntContext context)
        {
            return int.Parse(context.INT().GetText());
        }

        public override int VisitId([NotNull] ExprParser.IdContext context)
        {
            string id = context.ID().GetText();
            if (memory.ContainsKey(id)) return memory[id];
            return 0;
        }

        public override int VisitMulDiv([NotNull] ExprParser.MulDivContext context)
        {
            int left = Visit(context.expr(0));
            int right = Visit(context.expr(1));
            if (context.op.Type == ExprParser.MUL)
            {
                return left * right;
            }
            else
            {
                Debug.Assert(context.op.Type == ExprParser.DIV);
                return left / right;
            }
        }

        public override int VisitAddSub([NotNull] ExprParser.AddSubContext context)
        {
            int left = Visit(context.expr(0));
            int right = Visit(context.expr(1));
            if(context.op.Type == ExprParser.ADD)
            {
                return left + right;
            }
            else
            {
                Debug.Assert(context.op.Type == ExprParser.SUB);
                return left - right;
            }
        }

        public override int VisitParens([NotNull] ExprParser.ParensContext context)
        {
            return Visit(context.expr());
        }
    }
}
