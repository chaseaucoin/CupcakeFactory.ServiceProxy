using Amazon.Runtime;
using CupcakeFactory.ServiceProxy.Dispatchers;
using CupcakeFactory.ServiceProxy.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CupcakeFactory.ServiceProxy.Lambda
{
    public class LambdaDispatcher<TContract> : ProxyDispatcher<TContract>
    {
        private readonly string _functionName;
        private readonly string _region;
        private readonly ImmutableCredentials _credentials;
        private readonly ServiceMap _serviceMap;

        HttpClient _lambdaClient;

        public LambdaDispatcher(string functionName, string region, ImmutableCredentials credentials) : base(new JsonProxySerializer())
        {
            if (string.IsNullOrWhiteSpace(functionName))
            {
                throw new ArgumentException("Must include function name", nameof(functionName));
            }

            if (string.IsNullOrWhiteSpace(region))
            {
                throw new ArgumentException("Must include region", nameof(region));
            }

            _functionName = functionName;
            _region = region;
            _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
            _serviceMap = new ServiceMap();
            _serviceMap.RegisterContract<TContract>();

            _lambdaClient = new HttpClient();
        }

        /// <summary>
        /// Invokes the asynchronous generic.
        /// </summary>
        /// <typeparam name="TReturnType">The type of the return type.</typeparam>
        /// <param name="method">The method.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override async Task<TReturnType> InvokeAsyncGeneric<TReturnType>(MethodInfo method, object[] args)
        {
            //URI to invoke lambda function
            var requestUri = new Uri($"https://lambda.{_region}.amazonaws.com/2015-03-31/functions/{_functionName}/invocations");

            //Serialize the request
            var serializedRequest = SerializeRequest(method, args);
            var lambdaRequest = new LambdaRequest(_serviceMap.GetKey<TContract>(), serializedRequest);
            var lambdaRequestJosn = JsonConvert.SerializeObject(lambdaRequest);

            //Send request to lambda function
            var content = new StringContent(lambdaRequestJosn, Encoding.UTF8, "binary/octet-stream");
            var response = await _lambdaClient.PostAsync(requestUri, content, _region, "lambda", _credentials);
            var base64response = await response.Content.ReadAsStringAsync();
            
            //Get the serialized byte array
            var responseBytes = Convert.FromBase64String(base64response);

            //Deserialize the request and throw if exception
            var deserializedObject = _serializer.ResponseSerializer.DeserializeResponse<TReturnType>(method, responseBytes);

            return deserializedObject;
        }
    }
}
