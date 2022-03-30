using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace Language
{
    public static class Program
    {
        private static void Parse(string text)
        {
            try
            {
                //var text = File.ReadAllText(@"input.b");


                var input = new AntlrInputStream(text);
                Lexer lexer = new BLanguageLexer(input);
                lexer.RemoveErrorListeners();
                CommonTokenStream tokents = new CommonTokenStream(lexer);
                var parser = new BLanguageParser(tokents);
                parser.RemoveErrorListeners();
                //parser.AddErrorListener(DescriptiveErrorListener.Instance);
                var tree = parser.parse();
                var codegen = new CodeGeneratorVisitor();
                var result = codegen.Visit(tree);

            }catch(ParseCanceledException e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
        public static string ParseAndRun(string text)
        {
            Parse(text);
            return RunApp("HelloWrold.exe");
        }
        public static string RunApp(string exename)
        {

            using var process = new System.Diagnostics.Process();

            process.StartInfo.FileName = exename;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            StreamReader reader = process.StandardOutput;
            string output = reader.ReadToEnd();


            process.WaitForExit();


            return output;
        }

        public static void Run()
        {
            System.Diagnostics.Process.Start("HelloWrold.exe");
        }

        public static void BuildAndRun(string text)
        {
            Parse(text);
            Run();
        }
        private static void Main(string[] args)
        {
            var text = @$"
                A = 7 + 7 * 10 + 700;
                println ""sample is 7 7 7"";
                println A;
                ";
            BuildAndRun(text);

        }
    }
}