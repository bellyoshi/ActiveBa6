﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LL
{
    public class RecognitionExeption : Exception
    {
        public RecognitionExeption(string message) : base(message) { }
    }

}
