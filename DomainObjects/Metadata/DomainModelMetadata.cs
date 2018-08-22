using DomainObjects.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainObjects.Metadata
{
    public class DomainModelMetadata
    {
        readonly Dictionary<Type, DomainEntityMetadata> entityMetadata;
        readonly Dictionary<Type, DomainValueTypeMetadata> valueTypeMetadata;

        public string ModelName { get; }

        public IEnumerable<DomainEntityMetadata> GetAggregateRootDescriptors()
        {
            return entityMetadata.Values.Where(x => x.IsRoot);
        }

        public IEnumerable<DomainEntityMetadata> GetAggregateDescriptors()
        {
            return entityMetadata.Values.Where(x => !x.IsRoot);
        }

        public IEnumerable<DomainEntityMetadata> GetEntityDescriptors()
        {
            return entityMetadata.Values;
        }

        public bool TryGet(Type entityType, out DomainEntityMetadata domainEntityDescriptor)
        {
            return entityMetadata.TryGetValue(entityType, out domainEntityDescriptor);
        }

        public bool TryGet<T>(out DomainEntityMetadata domainEntityDescriptor)
            where T : DomainEntity
        {
            return entityMetadata.TryGetValue(typeof(T), out domainEntityDescriptor);
        }

        public DomainEntityMetadata GetType(Type entityType)
        {
            return entityMetadata[entityType];
        }

        public DomainEntityMetadata GetType<T>()
            where T : DomainEntity
        {
            return entityMetadata[typeof(T)];
        }

        public DomainModelMetadata(string modelName, IEnumerable<DomainEntityMetadata> entityMetadata, IEnumerable<DomainValueTypeMetadata> valueTypeMetadata)
        {
            ModelName = modelName;

            this.entityMetadata = entityMetadata.ToDictionary(x => x.EntityType);
            this.valueTypeMetadata = valueTypeMetadata.ToDictionary(x => x.Type);
        }
    }
}
