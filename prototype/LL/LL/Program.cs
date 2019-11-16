using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LL
{
    class Program
    {
        static void Main(string[] args)
        {
            a();
            b();
        }
        static void a()
        {
            Console.WriteLine("a");
            Parse("[abc,[jjj = hhh,kkk],dfghi ]");
        }

        static void b()
        {
            Console.WriteLine("b");
            Parse("[a,b]=[c,d]");
        }

        static void Parse(string src)
        {
            var lexer = new ListLexer(src);
            var parser = new ListParser(lexer);
            parser.Stat();
        }
    }
}
