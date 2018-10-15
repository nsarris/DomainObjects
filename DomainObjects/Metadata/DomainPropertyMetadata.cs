using DomainObjects.ModelBuilder.Descriptors;
using Dynamix.Reflection;
using System;

namespace DomainObjects.Metadata
{
    public abstract class DomainPropertyMetadata
    {
        public PropertyInfoEx Property { get; }
        public abstract DomainPropertyType DomainPropertyType { get; }
        public abstract bool IsList { get; }
        public abstract bool IsValueType { get; }
        public bool IsNullableType { get; }
        
        public Type Type => Property.Type;
        public string Name => Property.Name;


        internal DomainPropertyMetadata(PropertyDescriptor descriptor)
        {
            Property = descriptor.Property;
            IsNullableType = Property.Type.IsClass || Property.Type.IsNullable();
        }
    }
}
