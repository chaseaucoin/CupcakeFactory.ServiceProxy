﻿using CupcakeFactory.ServiceProxy.Models;
using CupcakeFactory.ServiceProxy.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CupcakeFactory.ServiceProxy.Invokers
{
    public class Invoker<TContract>
    {
        IProxySerializer _serializer;
        TContract _instance;
        public Invoker(IProxySerializer serializer, TContract instance)
        {
            _serializer = serializer;
            _instance = instance;
        }

        public async Task<Response> Invoke(byte[] serializedRequest)
        {
            var deserializedRequest = _serializer.RequestSerializer.DeserializeRequest<TContract>(serializedRequest);
            var response = await ContractParser<TContract>.Invoke(_instance, _serializer.ResponseSerializer, deserializedRequest.Method, deserializedRequest.Args.Values.ToArray());
            return response;
        }
    }
}
