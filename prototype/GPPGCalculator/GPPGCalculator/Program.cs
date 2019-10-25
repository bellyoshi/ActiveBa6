using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPPGCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new Calculator.CalculatorParser();
            do
            {
                Console.Write(">");
                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    return;
                }
                parser.Parse(input);
            } while (true);
        }
    }
}
