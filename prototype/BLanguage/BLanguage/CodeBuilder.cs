using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLanguage
{
    internal class CodeBuilder
    {
        public void Init()
        {
            LoadInstruction()
        }

        public void LoadInstruction(int space, string value)
        {
            AppendCodeLine(space, $"{ value}");
        }
    }
}
