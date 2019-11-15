using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LL
{
    class TokenTypes
    {
        public const int EOF_TYPE = 1;
        public const int NAME = 2;
        public const int COMMA = 3;
        public const int LBRACK = 4;
        public const int RBLACK = 5;
        public const int EQUAL = 6;

        private static string[] _tokenNames = { "n/a", "<EOF>", "NAME", "COMMA", "LBRACK", "RBRACK","EQUAL" };

        public  static String TokenName(int x)
        {
            return _tokenNames[x];
        }
    }
}
