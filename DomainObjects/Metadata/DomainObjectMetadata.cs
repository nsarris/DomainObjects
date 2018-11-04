using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainObjects.Metadata
{
    public class DomainObjectMetadata
    {
        public Type Type { get; }
        public bool IsShallow { get; }

        protected readonly Dictionary<string, DomainPropertyMetadata> propertyMetadata;

        public DomainObjectMetadata(Type type, IEnumerable<DomainPropertyMetadata> propertyMetadata)
        {
            Type = type;
            this.propertyMetadata = propertyMetadata.ToDictionary(x => x.Name);
            IsShallow = propertyMetadata.All(x => x is DomainValuePropertyMetadata v && v.DomainValueType != DomainValueType.ValueObject);
        }

        public bool ContainsProperty(string propertyName)
        {
            return propertyMetadata.ContainsKey(propertyName);
        }

        public bool TryGetProperty(string propertyName, out DomainPropertyMetadata propertyMetadata)
        {
            return this.propertyMetadata.TryGetValue(propertyName, out propertyMetadata);
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
    }
}
