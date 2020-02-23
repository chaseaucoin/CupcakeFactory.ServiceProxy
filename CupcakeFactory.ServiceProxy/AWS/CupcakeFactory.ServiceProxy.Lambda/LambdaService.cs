using CupcakeFactory.ServiceProxy.Invokers;
using CupcakeFactory.ServiceProxy.Models;
using CupcakeFactory.ServiceProxy.Serializers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CupcakeFactory.ServiceProxy.Lambda
{
    public class LambdaService
    {
        ServiceProvider _serviceProvider;
        ServiceMap _serviceMap;

        public LambdaService(ServiceProvider serviceProvider, ServiceMap serviceMap)
        {
            _serviceProvider = serviceProvider;
            _serviceMap = serviceMap;
        }

        public async Task<string> ExecuteRequest(LambdaRequest request)
        {
            var payloadBytes = Convert.FromBase64String(request.Base64Payload);
            var serviceType = _serviceMap.GetType(request.Service);
            
            var invokerType = typeof(Invoker<>).MakeGenericType(serviceType);
            var invoker = _serviceProvider.GetRequiredService(invokerType);

            var invokeMethod = invoker.GetType().GetMethod("Invoke");

            var responseTask = (Task<Response>)invokeMethod.Invoke(invoker, new object[] { payloadBytes });

            var response = await responseTask;

            var serializer = _serviceProvider.GetRequiredService<IProxySerializer>();

            var serializedResponse = serializer.ResponseSerializer.SerializeResponse(response);

            var result = Convert.ToBase64String(serializedResponse);

            return result;
        }
    }
}
