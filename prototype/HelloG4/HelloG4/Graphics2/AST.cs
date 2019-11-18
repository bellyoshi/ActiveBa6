using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics2
{
    class AST
    {
        Token token;
        List<AST> children;
        public AST(Token token)
        {
            this.token = token;
        }
        public void AddChild(AST t)
        {
            if (children == null) children = new List<AST>();
            children.Add(t);
        }
    }
}
