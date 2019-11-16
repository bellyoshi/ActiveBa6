using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LL
{
    class MismatchedTokenException : RecognitionExeption
    {
        public MismatchedTokenException(string message) : base(message) { }
    }
    
}
