using CupcakeFactory.ServiceProxy.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CupcakeFactory.ServiceProxy.Serializers
{
    public interface IRequestSerializer
    {
        byte[] SerializeRequest<TService>(Request request);

        Request DeserializeRequest<TService>(byte[] serializedRequest);

        byte[] SerializeRequest<TService>(MethodInfo method, object[] args);
    }

    public interface IResponseSerializer
    {
        byte[] SerializeResponse(Response response);

        TReturn DeserializeResponse<TReturn>(MethodInfo method, byte[] serializedResponse);
    }

    public interface IProxySerializer
    {
        IRequestSerializer RequestSerializer { get; }

        IResponseSerializer ResponseSerializer { get; }
    }
}