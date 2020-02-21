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

        public ProxyDispatcher(ISerializer serializer)
        {
            _serializer = serializer;
        }

        protected string SerializeRequest(MethodInfo method, object[] args)
        {
            var result = _serializer.RequestSerializer.SerializeRequest<TContract>(method, args);
            return result;
        }

        public abstract object Invoke(MethodInfo method, object[] args);

        public abstract Task InvokeAsync(MethodInfo method, object[] args);

        public abstract Task<T1> InvokeAsync<T1>(MethodInfo method, object[] args);
    }
}
