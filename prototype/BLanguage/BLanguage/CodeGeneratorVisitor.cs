using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace BLanguage
{
    public class CodeGeneratorVisitor : BLanguage.BLanguageBaseVisitor<Variable>
    {
        private readonly CodeBuilder _codeBuilder = new CodeBuilder();
        private string _mainMethod = "";
        private string _methodBody = "";
        private readonly string _moduleDefinition = "";

        private int _labelCount = 0;
        private string _lavelPrev = "";

        private Scope _currentScope = new Scope();
        private readonly List<Function> functions = new List<Function>();

        public CodeGeneratorVisitor()
        {
            _codeBuilder.Init();
            _moduleDefinition = _codeBuilder.GetCode();
        }

        public override Variable VisitParse([NotNull] BLanguageParser.ParseContext context)
        {
            var labelTo = _codeBuilder.MakeLabel(_labelCount);
            _labelCount++;
            Visit(context.block());
            _codeBuilder.LoadInstruction(2, _mainMethod);
            _codeBuilder.EmitTry(labelTo);
            _codeBuilder.EmitCatch(labelTo);
            _codeBuilder.LoadInstruction(2, _methodBody);
            var code = _moduleDefinition + _codeBuilder.GetCode() + "\n}";
            File.WriteAllText("out.il", code);
            return Variable.VOID;
        }

        public override Variable VisitPrintlnFunctionCall([NotNull] BLanguageParser.PrintlnFunctionCallContext context)
        {
            Visit(context.expression());
            _codeBuilder.EmitInBuiltFunctionCall("WriteLine",
                _codeBuilder.DataType[context.GetChild(2).GetText()]);
            return Variable.VOID;
        }

        public override Variable VisitPrintFunctionCall([NotNull] BLanguageParser.PrintFunctionCallContext context)
        {
            Visit(context.expression());
            _codeBuilder.EmitInBuiltFunctionCall("Write",
                _codeBuilder.DataType[context.GetChild(2).GetText()]);
            return Variable.VOID;
        }

        public override Variable VisitBlock([NotNull] BLanguageParser.BlockContext context)
        {
            foreach(var functionDecl in context.functionDecl())
            {
                this.Visit(functionDecl);
            }
            
            if(context.expression() == null)
            {
                return Variable.VOID;
            }
            var returnValue = Visit(context.expression());
            if(!Equals(returnValue, Variable.VOID)){
                _codeBuilder.LoadInstruction(2, OpCodes.Ret);
            }
            return returnValue;
        }

        public override Variable VisitVarDeclaration([NotNull] BLanguageParser.VarDeclarationContext context)
        {
            var varName = context.Identifier().GetText();
            var type = context.GetChild(0).GetText();
            _currentScope.AssignParameter(varName, new Variable(type));
            _codeBuilder.LoadInstruction(1,
                _codeBuilder.GetEmitLocals(varName, type));
            return new Variable(varName, type);
        }

    }
}
