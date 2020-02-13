using CupcakeFactory.ServiceProxy.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;

namespace CupcakeFactory.ServiceProxy.Serializers
{
    /// <summary>
    /// Serialized Requests into JSON
    /// </summary>
    /// <seealso cref="CupcakeFactory.ServiceProxy.Serializers.IRequestSerializer{System.String}" />
    public class JsonRequestSerializer : IRequestSerializer
    {
        /// <summary>
        /// Deserializes the request.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="serializedRequest">The serialized request.</param>
        /// <returns></returns>
        public Request DeserializeRequest<TService>(string serializedRequest)
        {
            JObject message = JObject.Parse(serializedRequest);
            var properties = message.Properties();
            var key = message.Properties().FirstOrDefault().Name;

            var method = ContractParser<TService>.GetMethod(key);

            object[] args = message.Value<JObject>(key)
                .Properties()
                .OrderBy(x => int.Parse(x.Name))
                .Select(x => x.Value.ToObject(ContractParser<TService>.GetPropertyType(key, x.Name)))
                .ToArray();

            return new Request(method, args);
        }

        /// <summary>
        /// Serializes the request.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public string SerializeRequest<TService>(Request request)
        {
            var methodKey = ContractParser<TService>.GetMethodKey(request.Method);

            JObject message = new JObject();
            message[methodKey] = new JObject();
            for (int i = 0; i < request.Args.Count; i++)
            {
                message[methodKey][i.ToString()] = JToken.FromObject(request.Args[i]);
            }

            return message.ToString();
        }

        public string SerializeRequest<TService>(MethodInfo method, object[] args) => SerializeRequest<TService>(new Request(method, args));
    }

    public class JsonResponseSerializer : IResponseSerializer
    {
        public object DeserializeResponse(Type returnType, string serializedObject)
        {
            JObject obj = JObject.Parse(serializedObject);

            if (obj["s"].Value<bool>())
            {
                var responseJson = obj["r"].ToString();
                return JsonConvert.DeserializeObject(responseJson, returnType);
            }
            else
            {
                var typeString = obj["r"]["t"].Value<string>();
                Type exceptionType = Type.GetType(typeString);
                if (exceptionType == null)
                    exceptionType = typeof(Exception);

                var exceptionJson = obj["r"]["e"].ToString();
                Exception exception = JsonConvert.DeserializeObject(exceptionJson, exceptionType) as Exception;

                throw exception;
            }
        }

        public object DeserializeResponse(MethodInfo methodInfo, string serializedObject)
        {
            return DeserializeResponse(methodInfo.ReturnType, serializedObject);
        }

        public TReturn DeserializeResponse<TReturn>(MethodInfo methodInfo, string serializedResponse)
        {
            return (TReturn)DeserializeResponse(typeof(TReturn), serializedResponse);
        }

        public string SerializeResponse(Response response)
        {
            return JsonConvert.SerializeObject(response);
        }
    }

    public class JsonSerializer<TContract> : ISerializer
    {
        IRequestSerializer _requestSerializer = new JsonRequestSerializer();

        IResponseSerializer _responseSerializer = new JsonResponseSerializer();
        
        IRequestSerializer ISerializer.RequestSerializer => _requestSerializer;
        
        IResponseSerializer ISerializer.ResponseSerializer => _responseSerializer;
    }
}
