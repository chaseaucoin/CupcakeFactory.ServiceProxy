using System.Collections.Generic;
using System.Reflection;

namespace CupcakeFactory.ServiceProxy.Models
{
    public class Request
    {
        public Request(MethodInfo method, object[] args)
        {
            Method = method;
            Args = new Dictionary<int, object>();

            for (int i = 0; i < args.Length; i++)
            {
                Args.Add(i, args[i]);
            }
        }
        public MethodInfo Method { get; set; }
        public Dictionary<int, object> Args { get; set; }
    }
}
