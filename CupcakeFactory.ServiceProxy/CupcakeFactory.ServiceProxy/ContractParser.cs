using CupcakeFactory.Extensions;
using CupcakeFactory.Extensions.Hash;
using CupcakeFactory.ServiceProxy.Models;
using CupcakeFactory.ServiceProxy.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CupcakeFactory.ServiceProxy
{
    public static class ContractParser<TService>
    {
        private static Type _serviceType;
        private static Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>();
        private static Dictionary<MethodInfo, string> _methodsReverse = new Dictionary<MethodInfo, string>();
        private static Dictionary<string, Dictionary<string, Type>> _parameterTypes = new Dictionary<string, Dictionary<string, Type>>();

        static ContractParser()
        {
            _serviceType = typeof(TService);            
            var methods = _serviceType.GetMethods();

            //Get a temp dictionary to hold full-length hashes
            var tempDictonary = new Dictionary<string, MethodInfo>();            

            foreach (var method in methods)
            {
                var hash = _getHash(method);
                tempDictonary.Add(hash, method);
            }

            //create smallest non-colliding hashes
            foreach(var method in tempDictonary)
            {
                List<char> charList = new List<char>();
                for (int i = 0; i < method.Key.Length; i++)
                {
                    charList.Add(method.Key[i]);

                    var key = new string(charList.ToArray());
                    if(tempDictonary.Count(x => x.Key.StartsWith(key)) == 1)
                    {
                        _methods.Add(key, method.Value);
                        _methodsReverse.Add(method.Value, key);

                        //get the paramaters and name them by order for easy lookup during deserialization
                        var parameterDictionary = method.Value
                            .GetParameters()
                            .ToDictionary(x => x.Position.ToString(), x => x.ParameterType);

                        _parameterTypes.Add(key, parameterDictionary);

                        break;
                    }
                }
            }
        }

        private static string _getHash(MethodInfo method)
        {
            var sb = new StringBuilder();
            sb.Append($"{_serviceType.FullName}.{method.Name}");

            foreach (var param in method.GetParameters().OrderBy(x => x.Name))
            {
                var paramName = param.Name;
                var typeName = param.ParameterType.FullName;

                sb.Append($"|{paramName}|{typeName}");
            }

            return sb.ToString().ToSHA256HashString();
        }

        public static string GetMethodKey(MethodInfo method) => _methodsReverse[method];

        public static MethodInfo GetMethod(string key) => _methods[key];

        public static Type GetPropertyType(string methodKey, string propertyKey) => _parameterTypes[methodKey][propertyKey];

        public static object DeserializeResponse(MethodInfo methodInfo, string serializedObject)
        {
            JObject obj = JObject.Parse(serializedObject);            

            if(obj["s"].Value<bool>())
            {
                var responseJson = obj["r"].ToString();
                return JsonConvert.DeserializeObject(responseJson, methodInfo.ReturnType);
            }
            else
            {
                var typeString = obj["r"]["t"].Value<string>();
                Type exceptionType = Type.GetType(typeString);
                if (exceptionType == null)
                    exceptionType = typeof(Exception);

                var exceptionJson = obj["r"]["e"].ToString();
                Exception exception = JsonConvert.DeserializeObject(exceptionJson, exceptionType) as Exception;

                throw exception;
            }
        }

        public static T DeserializeResponse<T>(MethodInfo methodInfo, string serializedObject)
        {
            return (T)DeserializeResponse(methodInfo, serializedObject);
        }

        public static Request DeserializeRequest(string serializedRequest)
        {
            JObject message = JObject.Parse(serializedRequest);
            var properties = message.Properties();
            var key = message.Properties().FirstOrDefault().Name;

            var method =  GetMethod(key);

            object[] args = message.Value<JObject>(key)
                .Properties()
                .OrderBy(x => int.Parse(x.Name))
                .Select(x => x.Value.ToObject(GetPropertyType(key,x.Name)))
                .ToArray();

            return new Request(method, args);
        }

        public static async Task<Response> Invoke(TService serviceInstance, 
            IResponseSerializer responseSerializer, 
            MethodInfo method,
            object[] args)
        {
            Response result = new Response();

            try
            {
                if (method.ReturnType == typeof(Task))
                {
                    Task task;

                    if (method.ReturnType == typeof(Task<>))
                    {
                        task = (Task)method.Invoke(serviceInstance, args);
                        await task
                            .ConfigureAwait(continueOnCapturedContext: false);

                        var resultProperty = task.GetType().GetProperty("Result");
                        result.ResponseObject = resultProperty.GetValue(task);
                    }
                    else
                    {
                        task = (Task)method.Invoke(serviceInstance, args);

                        await task
                            .ConfigureAwait(continueOnCapturedContext: false);
                    }
                }
                else if (method.ReturnType.Name == "Void")
                {
                    method.Invoke(serviceInstance, args);
                }
                else
                {
                    result.ResponseObject = method.Invoke(serviceInstance, args);
                }
            }
            catch(Exception ex)
            {
                result.Success = false;
                result.ResponseObject = new ExceptionWrapper(ex);
            }

            return result;
        }

        public static string SerializeMethodInput(MethodInfo method, params object[] args)
        {
            var methodKey = _methodsReverse[method];

            JObject message = new JObject();
            message[methodKey] = new JObject();
            for (int i = 0; i < args.Length; i++)
            {
                message[methodKey][i.ToString()] = JToken.FromObject(args[i]);
            }

            return message.ToString();
        }
    }
}
