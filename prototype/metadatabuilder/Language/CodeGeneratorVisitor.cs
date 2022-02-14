 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Language
{
    public class CodeGeneratorVisitor:BLanguageBaseVisitor<string>
    {
        ConsoleApp1.PEImageCreator PEImageCreator = new ConsoleApp1.PEImageCreator("Hello Wrold.exe");
    }
}
