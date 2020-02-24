using ExampleContracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExampleServices
{
    public class SubtractionService : ISubtractionService
    {
        public int Subtract(int x, int y)
        {
            return x - y;
        }
    }
}
