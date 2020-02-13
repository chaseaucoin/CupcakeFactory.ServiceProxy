﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CupcakeFactory.ServiceProxy.Tests
{
    public class ComplexObject
    {
        public string String { get; set; }

        public int Int { get; set; }

        public long Long { get; set; }

        public double Double { get; set; }

        public decimal Decimal { get; set; }

        public bool Bool { get; set; }

        public char Char { get; set; }

        public SimpleObject Child { get; set; }
    }
}
