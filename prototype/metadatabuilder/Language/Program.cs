using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace Language
{
    public static class Program
    {
        private static void Parse()
        {
            try
            {
                //var text = File.ReadAllText(@"input.b");
                var text = "println \"This is Sample\";\n" +
                            "println 200-50-50;\n" +
                            "println 1+2+3+4+5+6+7+8+9+10;\n" +
                            "println 8+3*4;\n" +
                            "abc = 10;\n" +
                            "bbb = 20;\n" +
                            "println abc + bbb;\n" +
                            "";

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
                System.Diagnostics.Process.Start("HelloWrold.exe");
            }catch(ParseCanceledException e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
        private static void Main(string[] args)
        {
            Parse();
        }
    }
}