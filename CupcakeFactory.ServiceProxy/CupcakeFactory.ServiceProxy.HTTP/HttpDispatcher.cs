using CupcakeFactory.ServiceProxy.Dispatchers;
using CupcakeFactory.ServiceProxy.Serializers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CupcakeFactory.ServiceProxy.HTTP
{
    public class HttpDispatcher<TContract> : ProxyDispatcher<TContract>
    {
        HttpClient _client;

        public HttpDispatcher(Uri endpoint) : base(new JsonProxySerializer())
        {
            _client = new HttpClient();
            _client.BaseAddress = endpoint;
        }

        public HttpDispatcher(HttpClient client) : base(new JsonProxySerializer())
        {
            _client = client;
        }

        public override async Task<TReturnType> InvokeAsyncGeneric<TReturnType>(MethodInfo method, object[] args)
        {
            //Serialize the request
            var serializedRequest = SerializeRequest(method, args);
            var serviceKey = typeof(TContract).FullName;

            //Send request to lambda function
            var content = new ByteArrayContent(serializedRequest);
            content.Headers.Add("X-ServiceProxy", serviceKey);

            var response = await _client.PostAsync(serviceKey, content);
            var responseBytes = await response.Content.ReadAsByteArrayAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"There was an error trying to invoke the function. Make sure credentials, roles, region, and the function name are all set correctly. \n{Encoding.UTF8.GetString(responseBytes)}");
            }

            //Deserialize the request and throw if exception
            var deserializedObject = _serializer.ResponseSerializer.DeserializeResponse<TReturnType>(method, responseBytes);

            return deserializedObject;
        }
    }
}
