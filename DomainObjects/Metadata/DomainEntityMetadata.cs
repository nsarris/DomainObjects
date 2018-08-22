using DomainObjects.Core;
using DomainObjects.ModelBuilder;
using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainObjects.Metadata
{
    public class DomainEntityMetadata
    {
        private readonly Dictionary<string, DomainPropertyMetadata> propertyMetadata;
        private readonly List<DomainValuePropertyMetadata> keyProperties;
        private readonly Func<DomainEntity, object> keySelector;
        //private readonly Dictionary<Type, DomainEntityMetadata> aggregateTypes;

        //public IReadOnlyDictionary<Type, DomainEntityMetadata> AggregateTypes => aggregateTypes;

        public DomainEntityMetadata(Type entityType, IEnumerable<DomainPropertyMetadata> propertyMetadata)
            //, IEnumerable<DomainEntityMetadata> aggregateTypes)
        {
            this.propertyMetadata = propertyMetadata.ToDictionary(x => x.Property.Name);
            keyProperties = this.propertyMetadata.Values.OfType<DomainValuePropertyMetadata>().Where(x => x.IsKeyMember).ToList();
            keySelector = DomainKeySelectorBuilder.BuildSelector(entityType, keyProperties.Select(x => x.Property.PropertyInfo).ToList());
            //this.aggregateTypes = aggregateTypes.ToDictionary(x => x.EntityType);

            EntityType = entityType;
            IsRoot = entityType.IsSubclassOfDeep(typeof(AggregateRoot));
        }

        public object GetKey(DomainEntity entity)
        {
            return keySelector(entity);
        }


        public DomainPropertyMetadata GetProperty(string propertyName)
        {
            return propertyMetadata[propertyName];
        }

        public IEnumerable<DomainPropertyMetadata> GetProperties()
        {
            return propertyMetadata.Values;
        }

        public DomainValuePropertyMetadata GetValueProperty(string propertyName)
        {
            var p = propertyMetadata[propertyName] as DomainValuePropertyMetadata;
            if (p is null)
                throw new KeyNotFoundException($"Property {propertyName} is not a value property");
            return p;
        }

        public IEnumerable<DomainValuePropertyMetadata> GetValueProperties()
        {
            return propertyMetadata.Values.OfType<DomainValuePropertyMetadata>();
        }

        public DomainValueListPropertyMetadata GetValueListProperty(string propertyName)
        {
            var p = propertyMetadata[propertyName] as DomainValueListPropertyMetadata;
            if (p is null)
                throw new KeyNotFoundException($"Property {propertyName} is not a value list");
            return p;
        }

        public IEnumerable<DomainValueListPropertyMetadata> GetValueListProperties()
        {
            return propertyMetadata.Values.OfType<DomainValueListPropertyMetadata>();
        }

        public DomainAggregatePropertyMetadata GetAggregateProperty(string propertyName)
        {
            var p = propertyMetadata[propertyName] as DomainAggregatePropertyMetadata;
            if (p is null)
                throw new KeyNotFoundException($"Property {propertyName} is not an aggregate property");
            return p;
        }

        public IEnumerable<DomainAggregatePropertyMetadata> GetAggregateProperties()
        {
            return propertyMetadata.Values.OfType<DomainAggregatePropertyMetadata>();
        }

        public DomainAggregateListPropertyMetadata GetAggregateListProperty(string propertyName)
        {
            var p = propertyMetadata[propertyName] as DomainAggregateListPropertyMetadata;
            if (p is null)
                throw new KeyNotFoundException($"Property {propertyName} is not a value list");
            return p;
        }

        public IEnumerable<DomainAggregateListPropertyMetadata> GetAggregateListProperties()
        {
            return propertyMetadata.Values.OfType<DomainAggregateListPropertyMetadata>();
        }

        public Type EntityType { get; }
        public bool IsRoot { get; }
    }
}
