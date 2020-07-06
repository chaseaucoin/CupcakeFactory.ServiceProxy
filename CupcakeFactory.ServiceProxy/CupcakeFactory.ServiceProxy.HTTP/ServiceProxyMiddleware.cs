using CupcakeFactory.ServiceProxy.Invokers;
using CupcakeFactory.ServiceProxy.Models;
using CupcakeFactory.ServiceProxy.Serializers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CupcakeFactory.ServiceProxy.HTTP
{
    public class ServiceProxyMiddleware<TContract>
    {
        Invoker<TContract> _invoker;
        IProxySerializer _serializer;

        public ServiceProxyMiddleware(Invoker<TContract> invoker, IProxySerializer serializer)
        {
            _invoker = invoker;
            _serializer = serializer;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var serviceProxyHeader = context.Request.Headers["X-ServiceProxy"];

            if (!string.IsNullOrEmpty(serviceProxyHeader) || serviceProxyHeader[0] != typeof(TContract).FullName)
            {
                byte[] payloadBytes = new byte[0];

                using (var requestMemoryStream = new MemoryStream())
                {
                    await context.Request.Body.CopyToAsync(requestMemoryStream);
                    payloadBytes = requestMemoryStream.ToArray();
                }
                var response = await _invoker.Invoke(payloadBytes);

                var serializedResponse = _serializer.ResponseSerializer.SerializeResponse(response);

                using (var responseMemoryStream = new MemoryStream(serializedResponse))
                {
                    await responseMemoryStream.CopyToAsync(context.Response.Body);
                }
            }
            else
            {
                await next(context);
            }
        }
    }
}
