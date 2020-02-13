using CupcakeFactory.ServiceProxy.Models;
using CupcakeFactory.ServiceProxy.Serializers;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CupcakeFactory.ServiceProxy
{
    public class ServiceProxy<TContract> : DispatchProxyAsync        
    {
        TContract _instance { get; set; }
        ISerializer _serializer { get; set; }

        public static TContract GetProxy(TContract instance, ISerializer serializer)
        {
            var proxy = Create<TContract, ServiceProxy<TContract>>();

            var adjustableProxy = proxy as ServiceProxy<TContract>;
            adjustableProxy._instance = instance;
            adjustableProxy._serializer = serializer;
            return proxy;
        }

        public override object Invoke(MethodInfo method, object[] args)
        {
            var request = _serializer.RequestSerializer.SerializeRequest<TContract>(method, args);
            var serializedInput = ContractParser<TContract>.SerializeMethodInput(method, args);

            var deserializedRequest = _serializer.RequestSerializer.DeserializeRequest<TContract>(request);

            Response response = null;
            ContractParser<TContract>.Invoke(_instance, _serializer.ResponseSerializer, deserializedRequest.Method, deserializedRequest.Args.Values.ToArray())
                .ContinueWith(x => response = x.Result)
                .Wait();

            var serializedResponse = _serializer.ResponseSerializer.SerializeResponse(response);
            var deserializedObject = _serializer.ResponseSerializer.DeserializeResponse(method, serializedResponse);

            return deserializedObject;
        }

        public override async Task InvokeAsync(MethodInfo method, object[] args)
        {
            //Serialize Request
            var request = _serializer.RequestSerializer.SerializeRequest<TContract>(new Request(method, args));

            //SendRequest to be invoked and get back serialized response
            var deserializedRequest = _serializer.RequestSerializer.DeserializeRequest<TContract>(request);
            var response = await ContractParser<TContract>.Invoke(_instance, _serializer.ResponseSerializer, deserializedRequest.Method, deserializedRequest.Args.Values.ToArray());

            //Deserialize Resposne            
            var serializedResponse = _serializer.ResponseSerializer.SerializeResponse(response);
            var deserializedObject = _serializer.ResponseSerializer.DeserializeResponse(method, serializedResponse);

            //If deserializedObject is exception throw exception
        }

        public override async Task<T1> InvokeAsyncT<T1>(MethodInfo method, object[] args)
        {
            //Serialize Request
            var request = _serializer.RequestSerializer.SerializeRequest<TContract>(new Request(method, args));

            //SendRequest to be invoked and get back serialized response
            var deserializedRequest = _serializer.RequestSerializer.DeserializeRequest<TContract>(request);
            var response = await ContractParser<TContract>.Invoke(_instance, _serializer.ResponseSerializer, deserializedRequest.Method, deserializedRequest.Args.Values.ToArray());

            //Deserialize Resposne            
            var serializedResponse = _serializer.ResponseSerializer.SerializeResponse(response);
            T1 deserializedObject = _serializer.ResponseSerializer.DeserializeResponse<T1>(method, serializedResponse);

            return deserializedObject;
        }
    }
}
