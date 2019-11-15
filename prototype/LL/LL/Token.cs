using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LL
{
    public class Token
    {
        public int Type { get; set; }
        public String Text{ get; set; }
        public Token(int type,string text)
        {
            this.Type = type;
            this.Text = text;
        }
        public override string ToString()
        {
            string tname = TokenTypes.TokenName(Type);
            return $"<'{Text}',{tname}>";
        }
    }
}
