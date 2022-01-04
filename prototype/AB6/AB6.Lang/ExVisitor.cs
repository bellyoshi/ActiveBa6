using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AB6.Lang
{
    internal class ExVisitor : HelloBaseVisitor<System.Linq.Expressions.Expression>
    {
        List<ParameterExpression> MemoryList = new List<ParameterExpression>();
        public override Expression VisitProg([NotNull] HelloParser.ProgContext context)
        {
            var expressions = new List<Expression>();
            foreach(var s in context.stat())
            {
                expressions.Add(Visit(s));
            }
            var block = Expression.Block(MemoryList, expressions);
            return block;
        }

        public override Expression VisitPrintExpr([NotNull] HelloParser.PrintExprContext context)
        {
            var expr =  Visit(context.expr());
            
            BlockExpression blockExpr = Expression.Block(
    Expression.Call(
        null,
        typeof(Console).GetMethod("WriteLine", new Type[] { typeof(int) }),
        expr
       ),
    Expression.Constant(0)
);
            return blockExpr;
        }
        public override Expression VisitAssign([NotNull] HelloParser.AssignContext context)
        {

            var name = context.ID().GetText();
            var left = MemoryList.Find(m => m.Name == name);

            if (left == null)
            {
                left = Expression.Parameter(typeof(int), name);
                MemoryList.Add(left);
            }
            var right = Visit(context.expr());

            return Expression.Assign(left, right);
        }
        private static Expression[] CreateParameterExpressions(MethodInfo method, params Expression[] argParam)
        {
            return method.GetParameters().Select((param, index) =>
                Expression.Convert(argParam[index]
              , param.ParameterType)).ToArray();
        }

        public override Expression VisitInt([NotNull] HelloParser.IntContext context)
        {
            return Expression.Constant(
                int.Parse(context.INT().GetText())
                );
        }

        public override Expression VisitId([NotNull] HelloParser.IdContext context)
        {
            string name = context.ID().GetText();
            return MemoryList.Find(m => m.Name == name);
            
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
