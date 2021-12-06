using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HelloG4
{
    internal class ExVisitor : HelloBaseVisitor<System.Linq.Expressions.Expression>
    {

        public override Expression VisitProg([NotNull] HelloParser.ProgContext context)
        {
            return base.VisitProg(context);
        }

        public override Expression VisitStat([NotNull] HelloParser.StatContext context)
        {
            return base.VisitStat(context);
        }


        public override Expression VisitInt([NotNull] HelloParser.IntContext context)
        {
            return Expression.Constant(
                int.Parse(context.INT().GetText())
                );
        }

        public override Expression VisitId([NotNull] HelloParser.IdContext context)
        {
            string id = context.ID().GetText();
            return Expression.Constant(0);//todo:
        }

        public override Expression VisitMulDiv([NotNull] HelloParser.MulDivContext context)
        {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));
            if (context.op.Type == HelloParser.MUL)
            {
                return Expression.Multiply( left, right);
            }
            else
            {

                return Expression.Divide(left, right);
            }
        }

        public override Expression VisitAddSub([NotNull] HelloParser.AddSubContext context)
        {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));
            if (context.op.Type == HelloParser.ADD)
            {
                return Expression.Add(left,  right);
            }
            else
            {

                return Expression.Subtract( left ,right);
            }
        }

        public override Expression VisitParens([NotNull] HelloParser.ParensContext context)
        {
            return Visit(context.expr());
        }




    }
}
