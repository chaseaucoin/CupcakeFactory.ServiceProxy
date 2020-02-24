using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using CupcakeFactory.ServiceProxy.Lambda;
using ExampleContracts;
using ExampleServices;
using Microsoft.Extensions.DependencyInjection;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace ExampleLambdaService
{
    public class Function
    {
        LambdaService _service;

        public Function()
        {
            //Split for readability
            LambdaServiceBuilder serviceBuilder = new LambdaServiceBuilder();

            //Configure any dependancies
            serviceBuilder = serviceBuilder
                .ConfigureDependencies(x =>
                {
                    x.AddSingleton<IAdditionService, AdditionService>();
                    x.AddSingleton<ISubtractionService, SubtractionService>();
                });

            //Add the proxy service to wire up the contracts
            //Only services that have been added will be exposed
            serviceBuilder = serviceBuilder
                .AddProxyService<IAdditionService>()
                .AddProxyService<ISubtractionService>();

            _service = serviceBuilder.Build();
        }


        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> FunctionHandler(LambdaRequest input, ILambdaContext context)
        {
            return await _service.ExecuteRequest(input);
        }
    }
}
