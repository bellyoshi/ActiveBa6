﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LL
{
    public class NoViableAltException: RecognitionExeption
    {
        public NoViableAltException(string message):base(message) { }
    }
}
