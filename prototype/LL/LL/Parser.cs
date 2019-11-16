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

        public Parser(Lexer input)
        {
            this._input = input;
       
            
        }
        public void Match(int x)
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
        public Token LT(int i)
        {
            Sync(i);
            return Lookahead[(p + i - 1)];
        }

        void Sync(int i)
        {
            if(p + i-1>(Lookahead.Count - 1))
            {
                int n = (p + i - 1) - (Lookahead.Count - 1);
                Fill(n);
            }
        }

        void Fill(int n)
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
        public void Consume()
        {
            p++;
            if(p==Lookahead.Count && !IsSpeculating())
            {
                p = 0;
                Lookahead.Clear();
            }
            Sync(1);
        }

        public bool IsSpeculating()
        {
            return 0 < markers.Count;
        }

        public int Mark()
        {
            markers.Add(p);
            return p;
        }
        public void Release()
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
