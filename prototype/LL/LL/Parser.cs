using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LL
{
    public class Parser
    {
        private Lexer _input;

        public Token[] Lookahead { get; private set; }
        private int k;
        private int p = 0;
       　
        public Parser(Lexer input, int k)
        {
            this._input = input;
            this.k = k;
            Lookahead = new Token[k];
            for(int i = 0; i < k; i++)
            {
                Consume();
            }
        }
        public void Match(int x)
        {
            if (LA(1) == x)
            {
                Consume();
            }
            else
            {
                throw new Exception($"expecting {TokenTypes.TokenName(x)}; found {Lookahead}");
            }
        }
        public Token LT(int i)
        {
            return Lookahead[(p + i - 1) % k];
        }
        public int LA(int i)
        {
            return LT(i).Type;
        }
        public void Consume()
        {
            Lookahead[p] = _input.NextToken();
            p = (p + 1) % k;
        }
    }
}
