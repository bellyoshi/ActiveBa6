using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var obj = typeof(System.Object);
            Console.WriteLine(obj.Assembly.FullName);
            Console.WriteLine(obj.Assembly.ImageRuntimeVersion);
            Console.WriteLine(obj.AssemblyQualifiedName);
        }
    }
}
