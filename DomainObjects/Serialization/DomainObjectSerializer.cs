using DomainObjects.Core;
using DomainObjects.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using Dynamix.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Serialization
{
    public class DomainObjectContractResolver : DefaultContractResolver
    {
        public Type GetActualTypeToCreate(Type objectType)
        {
            //Assert type
            if (objectType.IsSubclassOf(typeof(DomainEntity)))
                return ProxyTypeBuilder.BuildDomainEntityProxy(objectType);
            else if (objectType.IsSubclassOfGeneric(typeof(DomainValueObject<>)))
                return ProxyTypeBuilder.BuildSerializableProxy(objectType);
            else
                return objectType;
        }

        protected override JsonContract CreateContract(Type objectType)
        {
            return base.CreateContract(GetActualTypeToCreate(objectType));
        }
    }

    public class DomainSerializer
    {
        static readonly JsonSerializerSettings jsonSerializerSettings 
            = new JsonSerializerSettings() {
                ContractResolver = new DomainObjectContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

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
