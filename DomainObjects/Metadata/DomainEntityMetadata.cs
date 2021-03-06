﻿using DomainObjects.Core;
using DomainObjects.ModelBuilder;
using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainObjects.Metadata
{
    public class DomainEntityMetadata : DomainObjectMetadata
    {
        
        private readonly List<DomainValuePropertyMetadata> keyProperties;
        private readonly HashSet<string> ignoredProperties;
        private readonly Func<object, object> keySelector;
        private readonly Func<object, object> keyValueSelector;
        
        
        public bool IsRoot { get; }

        public DomainEntityMetadata(Type entityType, IEnumerable<DomainPropertyMetadata> properties, IEnumerable<string> ignoredProperties)
            :base(entityType, properties)
        {
            keyProperties = this.propertyMetadata.Values.OfType<DomainValuePropertyMetadata>().Where(x => x.IsKeyMember).ToList();
            keySelector = DomainKeySelectorBuilder.BuildKeySelector(entityType, keyProperties.Select(x => x.Property.PropertyInfo).ToList());
            keyValueSelector = DomainKeySelectorBuilder.BuildKeyValueSelector(entityType, keyProperties.Select(x => x.Property.PropertyInfo).ToList());
            this.ignoredProperties = new HashSet<string>(ignoredProperties);

            IsRoot = entityType.IsAggregateRoot();
        }

        public object GetKey(object entity)
        {
            return keySelector(entity);
        }

        public object GetKeyValue(object entity)
        {
            return keyValueSelector(entity);
        }

        public void SetKey(object entity, object value)
        {
            if (keyProperties.Count > 1)
                throw new InvalidOperationException($"Values given dont match the number of key properties in Entity {Type.Name}");

            keyProperties.First().Property.Set(entity, value);
        }

        public void SetKey(object entity, params object[] values)
        {
            //Primitives
            if (!keyProperties.Any(x => x.DomainValueType == DomainValueType.ValueObject))
            {
                if (values.Length != keyProperties.Count)
                    throw new InvalidOperationException($"Values given dont match the number of key properties in Entity {Type.Name}");

                foreach(var keyValue in keyProperties.Zip(values, (key, value) => new { key, value }))
                    keyValue.key.Property.Set(entity, keyValue.value);
            }
            //DomainValueObject
            else
            {
                var keyProperty = keyProperties.First();
                if (values.Length == 1 && values[0]?.GetType() == keyProperty.Type)
                    keyProperty.Property.Set(entity, values[0]);
                else
                {
                    var key = keyProperty.Type.GetConstructorsEx().Last().Invoke(values);
                    keyProperty.Property.Set(entity, key);
                }
            }
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

        public IEnumerable<DomainPropertyMetadata> GetKeyProperties() => keyProperties.AsEnumerable();

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

        public bool IsIgnored(string propertyName)
        {
            return ignoredProperties.Contains(propertyName);
        }
    }
}
