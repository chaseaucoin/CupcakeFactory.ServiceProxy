using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CupcakeFactory.ServiceProxy.Models
{
    public class ExceptionWrapper
    {
        public ExceptionWrapper()
        { }

        public ExceptionWrapper(Exception exception)
        {
            Exception = exception;

            var type = exception.GetType();
            var assembly = Assembly.GetAssembly(type);

            Type = $"{type.FullName}, {assembly.GetName().Name}";
        }

        [JsonProperty("t")]
        public string Type { get; set; }

        [JsonProperty("e")]
        public Exception Exception { get; set; }
    }
}
