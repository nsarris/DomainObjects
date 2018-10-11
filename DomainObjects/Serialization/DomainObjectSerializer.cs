using DomainObjects.Core;
using DomainObjects.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Serialization
{
    public class DomainObjectContractResolver : DefaultContractResolver
    {
        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var contract = base.CreateObjectContract(objectType);
            //Assert type
            if (objectType.IsSubclassOf(typeof(DomainEntity)))
                contract.CreatedType = ProxyTypeBuilder.BuildPropertyChangedProxy(objectType);

            return contract;
        }
    }

    public class DomainSerializer
    {
        static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings() { ContractResolver = new DomainObjectContractResolver(), ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

        public string Serialize(DomainObject domainObject)
        {
            return JsonConvert.SerializeObject(domainObject, jsonSerializerSettings);
        }

        public T Deserialize<T>(string serializedDomainObject)
            where T : DomainObject
        {
            return JsonConvert.DeserializeObject<T>(serializedDomainObject, jsonSerializerSettings);
        }

        public object Deserialize(Type domainObjectType, string serializedDomainObject)
        {
            //Assert type
            return JsonConvert.SerializeObject(serializedDomainObject, domainObjectType, jsonSerializerSettings);
        }
    }
}
