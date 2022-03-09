using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Language
{
    public class CodeGeneratorVisitor:BLanguageBaseVisitor<Expression>
    {
        ConsoleApp1.EmitHelper emit;
        ConsoleApp1.PEImageCreator PEImageCreator = new ConsoleApp1.PEImageCreator("HelloWrold.exe");
        ConsoleApp1.MetadataHelper metadataHelper;
        public override Expression VisitParse([NotNull] BLanguageParser.ParseContext context)
        {
            metadataHelper = PEImageCreator.metadataHelper;
            metadataHelper.s_guid = ConsoleApp1.PEImageCreator.s_guid;
            metadataHelper.AddModule("ConsoleApplication.exe")
                .AddAssembly("ConsoleApplication");

            emit = metadataHelper.GetEmit();
            //.ctor
            emit
                .ldarg_0
                .call(metadataHelper.Constructor())
                .ret
                .CtorDefinition();
            ;

            base.VisitParse(context);
            //Main
            var mainMethodDef =
             emit

                .ret
                .MethodDefinition("void", "Main");

            metadataHelper.AddTypeDefinition(mainMethodDef);

            var ret =  base.VisitParse(context);
            var entryPoint = mainMethodDef;
            PEImageCreator.Create(entryPoint);
            return ret;

        }
        public override Expression VisitPrintlnFunctionCall([NotNull] BLanguageParser.PrintlnFunctionCallContext context)
        {
            var expression = Visit(context.expression());
       
   

            emit.call("void", "System.Console.WriteLine", expression.typeName);
            return null;
        }
        public override Expression VisitStringExpression([NotNull] BLanguageParser.StringExpressionContext context)
        {
            var str = context.String().GetText();
            emit.ldstr(str);
            return new Expression() { typeName = "string"};
        }
    }
}
