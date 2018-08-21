using System;
using System.Reflection;

namespace DomainObjects.ModelBuilder
{
    public class EnumPropertyModelConfiguration : PropertyModelConfiguration
    {
        public Type UnderlyingType { get; }
        internal EnumPropertyModelConfiguration(PropertyInfo property) : base(property)
        {
            UnderlyingType = property.PropertyType.GetEnumUnderlyingType();
        }

        public new EnumPropertyModelConfiguration IsRequired()
        {
            base.IsRequired();
            return this;
        }
    }
}
