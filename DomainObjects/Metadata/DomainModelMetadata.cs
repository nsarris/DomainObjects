using DomainObjects.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainObjects.Metadata
{
    public class DomainModelMetadata
    {
        readonly Dictionary<Type, DomainEntityMetadata> entityMetadata;
        readonly Dictionary<Type, DomainValueObjectMetadata> valueTypeMetadata;

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

        public bool TryGetEntityMetadata(Type entityType, out DomainEntityMetadata domainEntityDescriptor)
        {
            return entityMetadata.TryGetValue(entityType, out domainEntityDescriptor);
        }

        public bool TryGetEntityMetadata<T>(out DomainEntityMetadata domainEntityDescriptor)
            where T : DomainEntity
        {
            return entityMetadata.TryGetValue(typeof(T), out domainEntityDescriptor);
        }

        public DomainEntityMetadata GetEntityMetadata(Type entityType)
        {
            return entityMetadata[entityType];
        }

        public DomainEntityMetadata GetEntityMetadata<T>()
            where T : DomainEntity
        {
            return entityMetadata[typeof(T)];
        }

        public bool TryGetValueObjectMetadata(Type entityType, out DomainValueObjectMetadata domainValueObjectDescriptor)
        {
            return valueTypeMetadata.TryGetValue(entityType, out domainValueObjectDescriptor);
        }

        public bool TryGetValueObjectMetadata<T>(out DomainValueObjectMetadata domainValueObjectDescriptor)
            where T : DomainObject
        {
            return valueTypeMetadata.TryGetValue(typeof(T), out domainValueObjectDescriptor);
        }

        public DomainValueObjectMetadata GetValueObjectMetadata(Type domainValueObjectType)
        {
            return valueTypeMetadata[domainValueObjectType];
        }

        public DomainValueObjectMetadata GetValueObjectMetadata<T>()
            where T : DomainEntity
        {
            return valueTypeMetadata[typeof(T)];
        }

        public DomainModelMetadata(string modelName, IEnumerable<DomainEntityMetadata> entityMetadata, IEnumerable<DomainValueObjectMetadata> valueTypeMetadata)
        {
            ModelName = modelName;

            this.entityMetadata = entityMetadata.ToDictionary(x => x.Type);
            this.valueTypeMetadata = valueTypeMetadata.ToDictionary(x => x.Type);
        }
    }
}
