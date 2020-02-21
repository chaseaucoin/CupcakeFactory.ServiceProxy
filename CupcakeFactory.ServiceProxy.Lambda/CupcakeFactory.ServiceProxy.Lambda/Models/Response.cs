using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CupcakeFactory.ServiceProxy.Models
{
    public class Response
    {
        public Response()
        {
            Success = true;
        }

        [JsonProperty("s")]
        public bool Success { get; set; }
        [JsonProperty("r")]
        public object ResponseObject { get; set; }
    }
}
