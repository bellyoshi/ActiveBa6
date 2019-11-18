using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LL
{
    public class Parser
    {
        Lexer _input;

        List<Token> Lookahead = new List<Token>();
        List<int> markers = new List<int>();
        int p = 0;

        protected Parser(Lexer input)
        {
            this._input = input;
       
            
        }
        protected void Match(int x)
        {
            if (LA(1) == x)
            {
                Consume();
            }
            else
            {
                throw new MismatchedTokenException($"expecting {TokenTypes.TokenName(x)}; found {LT(1)}");
            }
        }
        protected Token LT(int i)
        {
            Sync(i);
            return Lookahead[(p + i - 1)];
        }

        private void Sync(int i)
        {
            if(p + i-1>(Lookahead.Count - 1))
            {
                int n = (p + i - 1) - (Lookahead.Count - 1);
                Fill(n);
            }
        }

        private void Fill(int n)
        {
            
            for(int i = 0; i < n; i++)
            {
                Lookahead.Add(_input.NextToken());
            }
        }
        public int LA(int i)
        {
            return LT(i).Type;
        }
        protected void Consume()
        {
            p++;
            if(p==Lookahead.Count && !IsSpeculating())
            {
                p = 0;
                Lookahead.Clear();
            }
            Sync(1);
        }

        protected bool IsSpeculating()
        {
            return 0 < markers.Count;
        }

        protected int Mark()
        {
            markers.Add(p);
            return p;
        }
        protected void Release()
        {
            int marker = markers.Last();
            markers.Remove(marker);
            Seek(marker);
        }
        public void Seek(int index)
        {
            p = index;
        }
    }
}
