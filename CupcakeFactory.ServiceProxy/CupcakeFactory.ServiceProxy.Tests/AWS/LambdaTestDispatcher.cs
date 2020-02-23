using CupcakeFactory.ServiceProxy.Dispatchers;
using CupcakeFactory.ServiceProxy.Lambda;
using CupcakeFactory.ServiceProxy.Serializers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CupcakeFactory.ServiceProxy.Tests.AWS
{
    public class LambdaTestDispatcher<TContract> : ProxyDispatcher<TContract>
    {
        TContract _instance;
        LambdaService _lambdaService;
        ServiceMap _serviceMap;
        public LambdaTestDispatcher(TContract instance) : base(new JsonProxySerializer())
        {
            _instance = instance;

            LambdaServiceBuilder serviceBuilder = new LambdaServiceBuilder()
                .ConfigureDependencies(x =>
                {
                    x.AddSingleton(typeof(TContract), instance);
                })
                .AddProxyService<TContract>();

            _serviceMap = new ServiceMap();
            _serviceMap.RegisterContract<TContract>();

            _lambdaService = serviceBuilder.Build();
        }

        public override async Task<TReturnType> InvokeAsyncGeneric<TReturnType>(MethodInfo method, object[] args)
        {
            var serializedRequest = SerializeRequest(method, args);
            var lambdaRequest = new LambdaRequest(_serviceMap.GetKey<TContract>(), serializedRequest);

            var serializedResponse = await _lambdaService.ExecuteRequest(lambdaRequest);

            var responseBytes = Convert.FromBase64String(serializedResponse);

            //This would happen at the client
            var deserializedObject = _serializer.ResponseSerializer.DeserializeResponse<TReturnType>(method, responseBytes);

            return deserializedObject;
        }
    }
}
