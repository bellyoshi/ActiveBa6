﻿using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace BLanguage
{
    public static class Program
    {
        private static void Parse()
        {
            try
            {
                var text = File.ReadAllText(@"input.b");
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
        private static void Main(string[] args)
        {
            Parse();
        }
    }
}