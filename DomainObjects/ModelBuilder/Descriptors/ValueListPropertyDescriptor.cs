using DomainObjects.Metadata;
using Dynamix.Reflection;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.ModelBuilder.Descriptors
{
    sealed class ValueListPropertyDescriptor : CollectionPropertyDescriptor
    {
        public ValueListPropertyDescriptor(PropertyInfoEx property, Type elementType, DomainValueType domainValueType, bool isReadOnly) : base(property, elementType, isReadOnly)
        {
            DomainValueType = domainValueType;
        }

        public DomainValueType DomainValueType { get; }
    }
}
