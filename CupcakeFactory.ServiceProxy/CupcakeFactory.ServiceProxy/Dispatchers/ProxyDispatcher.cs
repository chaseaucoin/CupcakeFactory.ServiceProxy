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
        protected IProxySerializer _serializer;
        MethodInfo _dispatchMethod = typeof(ProxyDispatcher<TContract>).GetMethod(nameof(InvokeAsyncGeneric));
        
        public ProxyDispatcher(IProxySerializer serializer)
        {
            _serializer = serializer;
        }

        protected byte[] SerializeRequest(MethodInfo method, object[] args)
        {
            var result = _serializer.RequestSerializer.SerializeRequest<TContract>(method, args);
            return result;
        }

        public object Invoke(MethodInfo method, object[] args)
        {
            Task task = null;

            if (method.ReturnType.FullName == "System.Void")
                task = InvokeAsync(method, args);

            else
            {
                var genericDispathMethod = _dispatchMethod.MakeGenericMethod(method.ReturnType);
                task = (Task)genericDispathMethod.Invoke(this, new object[] { method, args });
            }

            object result = null;
            Exception e = null;

            task
                .ContinueWith(x => {
                    if (x.Status == TaskStatus.Faulted)
                        e = x.Exception.InnerException;

                    var resultProperty = x.GetType().GetProperty("Result");
                    
                    if (resultProperty != null)                        
                        result = resultProperty.GetValue(x);
                })
                .Wait();

            if (e != null)
                throw e.InnerException;

            return result;
        }

        public async Task InvokeAsync(MethodInfo method, object[] args)
        {
            var dispatchMethod = _dispatchMethod.MakeGenericMethod(typeof(object));
            var task = (Task)dispatchMethod.Invoke(this, new object[] { method, args });

            if (task.Status == TaskStatus.Faulted)
                throw task.Exception;

            await task;
        }

        public abstract Task<TReturnType> InvokeAsyncGeneric<TReturnType>(MethodInfo method, object[] args);
    }
}
