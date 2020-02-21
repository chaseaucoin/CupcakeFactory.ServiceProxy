using CupcakeFactory.ServiceProxy.Models;
using CupcakeFactory.ServiceProxy.Serializers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CupcakeFactory.ServiceProxy.Dispatchers
{
    public interface IDispatch
    {
        object Invoke(MethodInfo method, object[] args);

        Task InvokeAsync(MethodInfo method, object[] args);

        Task<TReturnType> InvokeAsyncGeneric<TReturnType>(MethodInfo method, object[] args);
    }
}
