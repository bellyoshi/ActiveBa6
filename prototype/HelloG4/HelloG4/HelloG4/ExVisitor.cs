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
        public override Expression VisitR([NotNull] HelloParser.RContext context)
        {
            var right = context.NUM();
            return Expression.Constant(0);
        }
        
        

    }
}
