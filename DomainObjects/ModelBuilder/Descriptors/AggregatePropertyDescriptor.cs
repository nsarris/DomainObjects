using Dynamix.Reflection;

namespace DomainObjects.ModelBuilder.Descriptors
{
    sealed class AggregatePropertyDescriptor : PropertyDescriptor
    {
        public AggregatePropertyDescriptor(PropertyInfoEx property) : base(property)
        {
        }
    }
}
