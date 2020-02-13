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
        string SerializeRequest<TService>(Request request);

        Request DeserializeRequest<TService>(string serializedRequest);

        string SerializeRequest<TService>(MethodInfo method, object[] args);
    }

    public interface IResponseSerializer
    {
        string SerializeResponse(Response response);

        object DeserializeResponse(MethodInfo method, string serializedResponse);
        TReturn DeserializeResponse<TReturn>(MethodInfo method, string serializedResponse);
    }

    public interface ISerializer
    {
        IRequestSerializer RequestSerializer { get; }

        IResponseSerializer ResponseSerializer { get; }
    }
}