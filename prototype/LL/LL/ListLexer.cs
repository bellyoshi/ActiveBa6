using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LL.TokenTypes;

namespace LL
{
    public class ListLexer : Lexer
    {


        public ListLexer(String input):base(input){ }

        public bool IsLetter {
            get
            {
                return 'a' <= c && c <= 'z' || 'A' <= c && c <= 'Z';
            }
        }

        void WS()
        {
            while (c == ' ' || c == '\t' || c == '\n' || c == '\r') Consume();
        }

        Token Name()
        {
            var buf = new System.Text.StringBuilder();
            do {
                buf.Append(c);
                Consume();
            } while (IsLetter);
            return new Token(NAME, buf.ToString());
        }

        public override Token NextToken() 
        {
            while(!IsEof)
            {
                switch (c)
                {
                    case ' ':
                    case '\t':
                    case '\n':
                    case '\r':
                        WS();continue;
                    case ',':
                        Consume();return new Token(COMMA, ",");
                    case '[':
                        Consume(); return new Token(LBRACK, "[");
                    case ']':
                        Consume();return new Token(RBLACK, "]");
                    case '=':
                        Consume();return new Token(EQUAL, "=");
                    default:
                        if (IsLetter) return Name();
                        throw new Exception("invalid caracter:" + c);
                }
            }
            return new Token(EOF_TYPE, "<EOF>");
        }


    }
}
