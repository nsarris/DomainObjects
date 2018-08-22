using System;
using Dynamix.Reflection;

namespace DomainObjects.ModelBuilder.Descriptors
{
    abstract class CollectionPropertyDescriptor : PropertyDescriptor
    {
        protected CollectionPropertyDescriptor(PropertyInfoEx property, Type elementType, bool isReadOnly) : base(property)
        {
            ElementType = elementType;
            IsReadOnly = isReadOnly;
        }

        public Type ElementType { get; }
        public bool IsReadOnly { get; }
    }
}
