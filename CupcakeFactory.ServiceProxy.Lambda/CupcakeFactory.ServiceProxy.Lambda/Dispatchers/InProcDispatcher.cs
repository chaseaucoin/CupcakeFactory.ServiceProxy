using CupcakeFactory.ServiceProxy.Invokers;
using CupcakeFactory.ServiceProxy.Models;
using CupcakeFactory.ServiceProxy.Serializers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CupcakeFactory.ServiceProxy.Dispatchers
{
    public class InProcDispatcher<TContract> : ProxyDispatcher<TContract>
    {
        TContract _instance;
        Invoker<TContract> _invoker;
        MethodInfo _dispatchMethod = typeof(InProcDispatcher<TContract>).GetMethod(nameof(InvokeAsyncGeneric));
        PropertyInfo _resultProperty = typeof(Task).GetProperty("Result");
        public InProcDispatcher(IProxySerializer serializer, TContract instance) : base(serializer)
        {
            _instance = instance;
            _invoker = new Invoker<TContract>(serializer, _instance);
        }

        public override async Task<TReturnType> InvokeAsyncGeneric<TReturnType>(MethodInfo method, object[] args)
        {
            var sw = Stopwatch.StartNew();

            var serializedRequest = SerializeRequest(method, args);

            var serializedRequestTime = sw.ElapsedMilliseconds;

            sw = Stopwatch.StartNew();

            //Would normally happen on another server            
            var response = await _invoker.Invoke(serializedRequest);

            var invokeTime = sw.ElapsedMilliseconds;

            sw = Stopwatch.StartNew();

            var serializedResponse = _serializer.ResponseSerializer.SerializeResponse(response);

            var serializeRepsonseTime = sw.ElapsedMilliseconds;

            sw = Stopwatch.StartNew();
            //This would happen at the client
            var deserializedObject = _serializer.ResponseSerializer.DeserializeResponse<TReturnType>(method, serializedResponse);

            var deserializeRsponseTime = sw.ElapsedMilliseconds;

            sw.Stop();

            return deserializedObject;
        }
    }
}
