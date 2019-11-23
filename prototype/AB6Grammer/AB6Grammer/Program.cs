using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;

namespace AB6Grammer
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputfilepath = null;
            if (0 < args.Length) inputfilepath = args[0];
            var ist = Console.In;
            if (inputfilepath != null) ist = new System.IO.StringReader(inputfilepath);
            var input = new AntlrInputStream(ist);
            var lexer = new AB6Lexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new AB6Parser(tokens);
            IParseTree tree = parser.prog();
            Console.ReadKey();
            var eval = new EvalVisitor();
            var cssrc = eval.Visit(tree);
            CompileCS(cssrc);
            Run();
            Console.ReadKey();
        }
        
        public static void Run()
        {
            Console.WriteLine();
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "hello.exe";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            Console.Write(process.StandardOutput.ReadToEnd());

        }

        public static void CompileCS(string cssrc)
        {
            var cscp = new CSharpCodeProvider();
            var param = new CompilerParameters()
            {
                OutputAssembly = "hello.exe",
                GenerateExecutable = true
            };
            var result = cscp.CompileAssemblyFromSource(param , new string[] { cssrc });
            if (0 < result.Errors.Count)
            {
                Console.Write(cssrc);
                Console.Write(result);
            }
        }
    }
}
