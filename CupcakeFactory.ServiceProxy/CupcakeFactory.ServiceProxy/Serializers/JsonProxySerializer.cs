using CupcakeFactory.ServiceProxy.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

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
        public Request DeserializeRequest<TService>(byte[] serializedRequest)
        {
            var stringMessage = Encoding.UTF8.GetString(serializedRequest);

            JObject message = JObject.Parse(stringMessage);
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
        public byte[] SerializeRequest<TService>(Request request)
        {
            var methodKey = ContractParser<TService>.GetMethodKey(request.Method);

            JObject message = new JObject();
            message[methodKey] = new JObject();
            for (int i = 0; i < request.Args.Count; i++)
            {
                message[methodKey][i.ToString()] = JToken.FromObject(request.Args[i]);
            }

            return Encoding.UTF8.GetBytes(message.ToString());
        }

        public byte[] SerializeRequest<TService>(MethodInfo method, object[] args) => SerializeRequest<TService>(new Request(method, args));
    }

    public class JsonResponseSerializer : IResponseSerializer
    {
        public TReturn DeserializeResponse<TReturn>(MethodInfo methodInfo, byte[] serializedResponse)
        {
            var stringMessage = Encoding.UTF8.GetString(serializedResponse);
            JObject obj = JObject.Parse(stringMessage);

            if (obj["s"].Value<bool>())
            {
                var responseJson = obj["r"].ToString();
                return JsonConvert.DeserializeObject<TReturn>(responseJson);
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

        public byte[] SerializeResponse(Response response)
        {
            var message = JsonConvert.SerializeObject(response);
            return Encoding.UTF8.GetBytes(message.ToString());
        }
    }

    public class JsonProxySerializer : IProxySerializer
    {
        IRequestSerializer _requestSerializer = new JsonRequestSerializer();

        IResponseSerializer _responseSerializer = new JsonResponseSerializer();
        
        IRequestSerializer IProxySerializer.RequestSerializer => _requestSerializer;
        
        IResponseSerializer IProxySerializer.ResponseSerializer => _responseSerializer;
    }
}
