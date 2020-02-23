using System;
using System.Collections.Generic;
using System.Text;

namespace ExampleLambdaService.Services
{
    public class AdditionService : IAdditionService
    {
        public int Add(int x, int y)
        {
            return x + y;
        }
    }
}
