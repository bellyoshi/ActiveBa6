using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Language
{
    public class CodeGeneratorVisitor:BLanguageBaseVisitor<string>
    {
        ConsoleApp1.EmitHelper emit;
        ConsoleApp1.PEImageCreator PEImageCreator = new ConsoleApp1.PEImageCreator("Hello Wrold.exe");
        public override string VisitParse([NotNull] BLanguageParser.ParseContext context)
        {
            return base.VisitParse(context);
        }
        public override string VisitPrintlnFunctionCall([NotNull] BLanguageParser.PrintlnFunctionCallContext context)
        {
            Visit(context.expression());
            emit.call("System.Console.WriteLine", "String","void");
            return String.Empty;
        }
    }
}
