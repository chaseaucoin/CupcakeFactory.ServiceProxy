using CupcakeFactory.ServiceProxy.Models;
using CupcakeFactory.ServiceProxy.Serializers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CupcakeFactory.ServiceProxy.Dispatchers
{
    public abstract class ProxyDispatcher<TContract> : IDispatch
    {
        protected ISerializer _serializer;
        MethodInfo _dispatchMethod = typeof(ProxyDispatcher<TContract>).GetMethod(nameof(InvokeAsyncGeneric));
        PropertyInfo _resultProperty = typeof(Task).GetProperty("Result");

        public ProxyDispatcher(ISerializer serializer)
        {
            _serializer = serializer;
        }

        protected string SerializeRequest(MethodInfo method, object[] args)
        {
            var result = _serializer.RequestSerializer.SerializeRequest<TContract>(method, args);
            return result;
        }

        public object Invoke(MethodInfo method, object[] args)
        {
            var task = (Task)_dispatchMethod.Invoke(this, new object[] { method, args });
            object result = null;

            task
                .ContinueWith(x => {
                    result = _resultProperty.GetValue(x);
                })
                .Wait();

            return result;
        }

        public async Task InvokeAsync(MethodInfo method, object[] args)
        {
            var task = (Task)_dispatchMethod.Invoke(this, new object[] { method, args });
            await task;
        }

        public abstract Task<T1> InvokeAsyncGeneric<T1>(MethodInfo method, object[] args);
    }
}
