using CupcakeFactory.ServiceProxy.Invokers;
using CupcakeFactory.ServiceProxy.Models;
using CupcakeFactory.ServiceProxy.Serializers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CupcakeFactory.ServiceProxy.Dispatchers
{
    public class InProcDispatcher<TContract> : ProxyDispatcher<TContract>
    {
        TContract _instance;
        Invoker<TContract> _invoker;
        public InProcDispatcher(ISerializer serializer, TContract instance) : base(serializer)
        {
            _instance = instance;
            _invoker = new Invoker<TContract>(serializer, _instance);
        }

        public override object Invoke(MethodInfo method, object[] args)
        {
            var serializedRequest = SerializeRequest(method, args);

            //Would normally happen on another server
            Response response = null;
            _invoker.Invoke(serializedRequest)
                .ContinueWith(x => response = x.Result)
                .Wait();

            var serializedResponse = _serializer.ResponseSerializer.SerializeResponse(response);
            
            //This would happen at the client
            var deserializedObject = _serializer.ResponseSerializer.DeserializeResponse(method, serializedResponse);

            return deserializedObject;
        }

        public override async Task InvokeAsync(MethodInfo method, object[] args)
        {
            var serializedRequest = SerializeRequest(method, args);

            //Would normally happen on another server            
            var response = await _invoker.Invoke(serializedRequest);

            var serializedResponse = _serializer.ResponseSerializer.SerializeResponse(response);

            //This would happen at the client
            _serializer.ResponseSerializer.DeserializeResponse(method, serializedResponse);
        }

        public override async Task<T1> InvokeAsync<T1>(MethodInfo method, object[] args)
        {
            var serializedRequest = SerializeRequest(method, args);

            //Would normally happen on another server            
            var response = await _invoker.Invoke(serializedRequest);

            var serializedResponse = _serializer.ResponseSerializer.SerializeResponse(response);

            //This would happen at the client
            var deserializedObject = _serializer.ResponseSerializer.DeserializeResponse<T1>(method, serializedResponse);

            return deserializedObject;
        }
    }
}
