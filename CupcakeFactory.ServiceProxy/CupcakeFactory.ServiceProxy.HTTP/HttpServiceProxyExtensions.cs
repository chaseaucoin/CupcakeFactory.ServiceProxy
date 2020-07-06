using CupcakeFactory.ServiceProxy.HTTP;
using CupcakeFactory.ServiceProxy.Invokers;
using CupcakeFactory.ServiceProxy.Serializers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace CupcakeFactory.ServiceProxy
{
    public static class HttpServiceProxyExtensions
    {
        public static TContract GetProxy<TContract>(this HttpClient client)
        {
            var dispatcher = new HttpDispatcher<TContract>(client);
            return ServiceProxy<TContract>.GetProxy(dispatcher);
        }

        public static IApplicationBuilder AddProxy<TContract>(this IApplicationBuilder builder)
        {
            builder.Use(next =>
            {
                return context =>
                {
                    var serviceProxyHeader = context.Request.Headers["X-ServiceProxy"];

                    if (!string.IsNullOrEmpty(serviceProxyHeader) || serviceProxyHeader[0] != typeof(TContract).FullName)
                    {
                        var service = builder.ApplicationServices.GetService<TContract>();
                        var serializer = new JsonProxySerializer();
                        var invoker = new Invoker<TContract>(serializer, service);
                        var middleware = new ServiceProxyMiddleware<TContract>(invoker, serializer);
                        return middleware.InvokeAsync(context, next);
                    }
                    else
                    {
                        return next(context);
                    }
                };
            });

            return builder;
        }
    }
}


