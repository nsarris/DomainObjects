using DomainObjects.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainObjects.Metadata
{
    public class DomainModelMetadata
    {
        readonly Dictionary<Type, DomainEntityMetadata> entityDescriptors = new Dictionary<Type, DomainEntityMetadata>();
        bool isBuilt;

        public IEnumerable<DomainEntityMetadata> GetAggregateRootDescriptors()
        {
            return entityDescriptors.Values.Where(x => x.IsRoot);
        }

        public IEnumerable<DomainEntityMetadata> GetAggregateDescriptors()
        {
            return entityDescriptors.Values.Where(x => !x.IsRoot);
        }

        public IEnumerable<DomainEntityMetadata> GetEntityDescriptors()
        {
            return entityDescriptors.Values;
        }

        public bool TryGet(Type entityType, out DomainEntityMetadata domainEntityDescriptor)
        {
            return entityDescriptors.TryGetValue(entityType, out domainEntityDescriptor);
        }

        public bool TryGet<T>(out DomainEntityMetadata domainEntityDescriptor)
            where T : DomainEntity
        {
            return entityDescriptors.TryGetValue(typeof(T), out domainEntityDescriptor);
        }

        public DomainEntityMetadata GetType(Type entityType)
        {
            return entityDescriptors[entityType];
        }

        public DomainEntityMetadata GetType<T>()
            where T : DomainEntity
        {
            return entityDescriptors[typeof(T)];
        }

        public void RegisterType(Type entityType)
        {
            if (isBuilt)
                throw new InvalidOperationException("No new entities can be added after the model has been built");

            var descriptor = new DomainEntityMetadata(entityType);

            if (!descriptor.IsRoot)
                throw new InvalidOperationException("Only root entities can be registered in mode descriptor");

            entityDescriptors.Add(entityType, descriptor);
        }

        public void BuildModel()
        {
            if (isBuilt)
                return;

            foreach (var descriptor in entityDescriptors.Values)
            {
                descriptor.Build(null);
                foreach (var aggregateDescriptor in descriptor.AggregateTypes.Values.SelectManyRecursive(x => x.AggregateTypes.Values))
                {
                    aggregateDescriptor.Build(null);
                    entityDescriptors.Add(aggregateDescriptor.EntityType, aggregateDescriptor);
                }
            }

            DomainModelMetadataRegistry.RegisterModel(this);

            isBuilt = true;
        }
    }
}
