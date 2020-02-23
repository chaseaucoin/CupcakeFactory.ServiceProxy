using CupcakeFactory.Extensions.Hash;
using System;
using System.Collections.Generic;
using System.Text;

namespace CupcakeFactory.ServiceProxy
{
    public class ServiceMap
    {
        private Dictionary<string, Type> _index = new Dictionary<string, Type>();
        private Dictionary<Type, string> _reverseIndex = new Dictionary<Type, string>();

        public void RegisterContracts(IEnumerable<Type> serviceContracts)
        {
            foreach (var serviceContract in serviceContracts)
            {
                RegisterContract(serviceContract);
            }
        }

        public void RegisterContract<TContract>()
        {
            RegisterContract(typeof(TContract));
        }

        public void RegisterContract(Type serviceContract)
        {
            string hash = serviceContract.FullName.ToSHA256ShortHash();

            _index.Add(hash, serviceContract);
            _reverseIndex.Add(serviceContract, hash);
        }

        public string GetKey<TContract>() => _reverseIndex[typeof(TContract)];

        public Type GetType(string key) => _index[key];
    }
}