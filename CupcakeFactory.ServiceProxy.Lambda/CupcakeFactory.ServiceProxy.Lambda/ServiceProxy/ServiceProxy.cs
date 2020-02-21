using CupcakeFactory.ServiceProxy.Dispatchers;
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
        IDispatch _dispatcher { get; set; }

        public static TContract GetProxy(IDispatch dispatcher)
        {
            var proxy = Create<TContract, ServiceProxy<TContract>>();

            var adjustableProxy = proxy as ServiceProxy<TContract>;
            adjustableProxy._dispatcher = dispatcher;

            return proxy;
        }

        public override object Invoke(MethodInfo method, object[] args)
        {
            var result = _dispatcher.Invoke(method, args);
            return result;
        }

        public override async Task InvokeAsync(MethodInfo method, object[] args)
        {
            await _dispatcher.InvokeAsync(method, args);
        }

        public override async Task<T1> InvokeAsyncT<T1>(MethodInfo method, object[] args)
        {
            var result = await _dispatcher.InvokeAsync<T1>(method, args);
            return result;
        }
    }
}
