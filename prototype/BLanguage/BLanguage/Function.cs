using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLanguage
{
    public class Function
    {
        public string Name { get; set; }

        public List<Variable> Arguments { get; set; } = new List<Variable>();
    }
}
