using Dynamix.Reflection;
using System;

namespace DomainObjects.ModelBuilder.Descriptors
{
    sealed class AggregateListPropertyDescriptor : CollectionPropertyDescriptor
    {
        public AggregateListPropertyDescriptor(PropertyInfoEx property, Type elementType, bool isReadOnly) : base(property, elementType, isReadOnly)
        {
        }
    }
}
