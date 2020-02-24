using ExampleContracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExampleServices
{
    public class AdditionService : IAdditionService
    {
        public int Add(int x, int y)
        {
            return x + y;
        }
    }
}
