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
                var text = "println \"This is Sample\"\n" +
                            "println 200\n" +
                            "println \"Hello ANTLR4\"\n";
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