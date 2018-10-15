using DomainObjects.Metadata;
using Dynamix.Reflection;

namespace DomainObjects.ModelBuilder.Descriptors
{
    sealed class ValuePropertyDescriptor : PropertyDescriptor
    {
        public ValuePropertyDescriptor(PropertyInfoEx property, DomainValueType domainValueType) : base(property)
        {
            DomainValueType = domainValueType;
        }

        public DomainValueType DomainValueType { get; }
    }
}
