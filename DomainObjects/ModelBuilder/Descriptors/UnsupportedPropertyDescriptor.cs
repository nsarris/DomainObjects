using Dynamix.Reflection;

namespace DomainObjects.ModelBuilder.Descriptors
{
    sealed class UnsupportedPropertyDescriptor : PropertyDescriptor
    {
        public UnsupportedPropertyDescriptor(PropertyInfoEx property) : base(property)
        {
        }
    }
}
