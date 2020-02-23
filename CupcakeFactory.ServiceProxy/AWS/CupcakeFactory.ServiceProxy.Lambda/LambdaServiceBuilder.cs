using CupcakeFactory.ServiceProxy.Invokers;
using CupcakeFactory.ServiceProxy.Serializers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CupcakeFactory.ServiceProxy.Lambda
{
    public class LambdaServiceBuilder
    {
        IServiceCollection _serviceContainer;
        ServiceMap _serviceMap = new ServiceMap();
        public LambdaServiceBuilder() : this(new ServiceCollection()) { }

        public LambdaServiceBuilder(IServiceCollection serviceContainer)
        {
            _serviceContainer = serviceContainer;
            _serviceContainer.AddSingleton<IProxySerializer, JsonProxySerializer>();
        }

        public LambdaServiceBuilder ConfigureDependencies(Action<IServiceCollection> serviceConfiguration)
        {
            serviceConfiguration(_serviceContainer);
            return this;
        }

        public LambdaServiceBuilder AddProxyService<TContract>()
        {
            _serviceContainer.AddTransient<Invoker<TContract>>(x => {
                var serializer = x.GetRequiredService<IProxySerializer>();
                var instance = x.GetRequiredService<TContract>();
                var invoker = new Invoker<TContract>(serializer, instance);

                return invoker;
            });

            _serviceMap.RegisterContract(typeof(TContract));

            return this;
        }

        public LambdaService Build()
        {
            var serviceProvider = _serviceContainer.BuildServiceProvider();

            return new LambdaService(serviceProvider, _serviceMap);
        }
    }
}
