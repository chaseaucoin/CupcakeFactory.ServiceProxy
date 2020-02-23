using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CupcakeFactory.ServiceProxy.Lambda
{
    public class LambdaRequest
    {
        public LambdaRequest() { }

        public LambdaRequest(string service, byte[] payloadBytes)
        {
            Service = service;
            Base64Payload = Convert.ToBase64String(payloadBytes);
        }

        [JsonProperty("s")]
        public string Service { get; set; }

        [JsonProperty("p")]
        public string Base64Payload { get; set; }
    }
}
