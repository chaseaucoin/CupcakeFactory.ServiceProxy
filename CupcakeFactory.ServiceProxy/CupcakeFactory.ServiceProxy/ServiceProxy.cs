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
            try
            {
                var result = _dispatcher.Invoke(method, args);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override async Task InvokeAsync(MethodInfo method, object[] args)
        {
            try
            {
                await _dispatcher.InvokeAsync(method, args);
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override async Task<T1> InvokeAsyncT<T1>(MethodInfo method, object[] args)
        {
            var result = await _dispatcher.InvokeAsyncGeneric<T1>(method, args);
            return result;
        }
    }
}
