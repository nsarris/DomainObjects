using Dynamix.Reflection;

namespace DomainObjects.ModelBuilder.Descriptors
{
    internal abstract class PropertyDescriptor
    {
        protected PropertyDescriptor(PropertyInfoEx property)
        {
            Property = property;
        }

        public PropertyInfoEx Property { get; }
    }
}
