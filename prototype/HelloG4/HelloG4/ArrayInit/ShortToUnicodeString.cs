using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;

namespace ArrayInit
{
    class ShortToUnicodeString : ArrayInitBaseListener
    {
        public override void EnterInit([NotNull] ArrayInitParser.InitContext context)
        {
            base.EnterInit(context);
            Console.Write('"');
        }
        public override void ExitInit([NotNull] ArrayInitParser.InitContext context)
        {
            base.ExitInit(context);
            Console.Write('"');
        }
        public override void EnterValue([NotNull] ArrayInitParser.ValueContext context)
        {
            int value = int.Parse(context.INT().GetText());
            Console.Write(@"\u{0: X4}", value);
            base.EnterValue(context);
        }
    }
}
