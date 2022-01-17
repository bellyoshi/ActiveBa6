using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;

namespace BLanguage
{
    public class CodeBuilder
    {
        private readonly StringBuilder _result = new StringBuilder();
        public void Append(string code) => _result.Append(code);
        private Dictionary<string, string> _dataType = new Dictionary<string, string>()
        {
            {"int", "int32" },
            {"float", "float32" },
            {"double", "float64" },
            {"string", "string" },
            {"bool", "bool" },
            {"math","System.Random" },
            {"%d", "int32" },
            {"%f", "float32" },
            {"%s", "string" }
        };
        public Dictionary<string, string> DataType => _dataType;

        public void Init()
        {
            LoadInstruction(1, ".assembly extern mscorlib\n{\n}\n");
            LoadInstruction(1, ".assembly " + "Program" + "\n{\n}\n\n.module" + "test" + ".exe\n");
            LoadInstruction(1, ".class private auto ansi beforefieldinit Program extends [System.Runtime]System.Object ");
            LoadInstruction(1, "{\n");
            LoadInstruction(1, ".method private hidebysig static void Main(string[] args) cil managed {");
            LoadInstruction(1, ".entrypoint");
            LoadInstruction(1, ".maxstack 8");
            LoadInstruction(1, ".local init (class[System.Net.Http]System.Net.Http.HttpClient client)");
            LoadInstruction(1, ".local init (class[mscorlib]System.Exception e)");
            LoadInstruction(1, ".try");
            LoadInstruction(1, "{");
        }

        public void EmitTry(string lavelTo)
        {
            AppendCodeLine(2, "nop");
            AppendCodeLine(2, "leave.s " + lavelTo);
            AppendCodeLine(1, "}");
        }

        public void EmitCatch(string lavelTo) {
            AppendCodeLine(1, "catch [mscorlib]System.Exception");
            AppendCodeLine(1, "{");
            AppendCodeLine(3, "stloc.0");
            AppendCodeLine(3, " nop");
            AppendCodeLine(3, "ldloc.0");
            AppendCodeLine(3, "callvirt instance string [mscorlib]System.Exception::get_Message()");
            AppendCodeLine(3, "call void [System.Console]System.Console::WriteLine(string)");
            AppendCodeLine(3, "nop");
            AppendCodeLine(3, "nop");
            AppendCodeLine(3, "leave.s " + lavelTo);
            AppendCodeLine(3, "}");
            AppendCodeLine(3, $"{lavelTo}: ret");
            AppendCodeLine(1, "}");
            
        }

        private void AppendCodeLine(int pos, string code)
        {
            for(int i = 0; i < pos; i++)
            {
                _result.Append("\t");
            }
            _result.Append(code);
        }

        public void InitializeVariable(string variableName, string value)
        {
            Append($"stloca {variableName}\n");
            Append($"ldc.i4 {value}\n");
            Append($"stloc {variableName}\n");
        }

        public string MakeLabel(int label)
        {
            return string.Format("IL_{0:x4}", label);
        }

        public void LoadInstruction(int space, string opcode, string value)
        {
            AppendCodeLine(space, $"{opcode} {value}");
        }

        public void LoadInstruction(int space, string value)
        {
            AppendCodeLine(space, $"{ value}");
        }

        public void  EmitInBuiltFunctionCall(string type)
        {
            AppendCodeLine(2, $"call void [mscorlib]System.Console::WriteLine({type})");
        }
        public string GetCode()
        {
            var result = _result.ToString();    
            _result.Clear();
            return result;
        }

        public void BuildMethod(string[] types, string[] parameters, string methodName, string returnType = "void")
        {
            Contract.Requires(types.Length == parameters.Length);
            var methodBody = string.Empty;
            returnType = _dataType[returnType];
            methodBody += $".method private hidebysig static {returnType} {methodName} (";
            for(int i = 0; i < types.Length; i++)
            {
                methodBody += $"{types[i]} {parameters[i]}";
                if(i < parameters.Length - 1)
                {
                    methodBody += ",";
                }
            }
            methodBody += ") cil managed";
            AppendCodeLine(1, methodBody + "{");
        }

        public string GetEmitLocals(string [] types, params string[] parameters)
        {
            Contract.Requires(types.Length == parameters.Length);
            Contract.Requires(types.Any(c => _dataType.ContainsKey(c)));
            string localInit = ".locals init ( ";
            for(int i = 0; i < parameters.Length; i++)
            {
                var type = types[i];
                localInit += $"{type} {parameters[i]}";
                if (i < parameters.Length - 1)
                {
                    localInit += ", ";
                }
            }
            return localInit + ")";
        }

        public string GetEmitLocals(string parameter, string type)
        {
            var datatype = _dataType[type];
            return GetEmitLocals(new string[] { datatype }, new string[] { parameter });
        }

        public void EmitHttpClientStart(string identifier)
        {
            AppendCodeLine(1, "nop");
            AppendCodeLine(1, $"ldloc {identifier}");
        }

        public void EmitHttpClientEnd(string identidier)
        {
            AppendCodeLine(2, "call string [System.IO.FileSystem]System.IO.File::ReadAllText(string)");
            AppendCodeLine(2, $"stloc {identidier}");
            AppendCodeLine(2, $"ldloc {identidier}");
            AppendCodeLine(2, "call void[System.Console]System.Console::WriteLine(string)");
            AppendCodeLine(2, "nop");
        }
    }
}
