using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainObjects.Metadata
{
    public class DomainObjectMetadata
    {
        public Type Type { get; }

        protected readonly Dictionary<string, DomainPropertyMetadata> propertyMetadata;

        public DomainObjectMetadata(Type type, IEnumerable<DomainPropertyMetadata> propertyMetadata)
        {
            Type = type;
            this.propertyMetadata = propertyMetadata.ToDictionary(x => x.Name);
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
