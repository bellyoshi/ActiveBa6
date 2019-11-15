using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LL
{
    public abstract class Lexer
    {
        public bool IsEof
        {
            get
            {
                return input.Length <= p;
            }
        }

        private string input;
        private int p = 0;
        public char c {
            get {
                if (IsEof) return '\0';
                return input[p];
            }
        }
        public Lexer(String input)
        {
            this.input = input;
        }
        public void Consume()
        {
            p++;
        }
        public void match(char x)
        {
            if (c == x) Consume();
            else throw new Exception($"expecting {x};found {c}");
        }

        public abstract Token NextToken();

    }
}
