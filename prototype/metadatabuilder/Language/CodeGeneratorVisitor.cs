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
        ConsoleApp1.PEImageCreator PEImageCreator = new ConsoleApp1.PEImageCreator("HelloWrold.exe");
        ConsoleApp1.MetadataHelper metadataHelper;
        public override string VisitParse([NotNull] BLanguageParser.ParseContext context)
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
        public override string VisitPrintlnFunctionCall([NotNull] BLanguageParser.PrintlnFunctionCallContext context)
        {
            Visit(context.expression());
            emit.ldstr("Hello AAA")
            .call(metadataHelper.ConsoleWriteLine());

            //emit.call("System.Console.WriteLine", "String","void");
            return String.Empty;
        }
    }
}
