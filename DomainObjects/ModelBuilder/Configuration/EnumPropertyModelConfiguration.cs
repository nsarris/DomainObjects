using System;
using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public class EnumPropertyModelConfiguration : PropertyModelConfiguration
    {
        public Type UnderlyingType { get; }
        internal EnumPropertyModelConfiguration(PropertyInfo property) : base(property)
        {
            UnderlyingType = property.PropertyType.GetEnumUnderlyingType();
        }

        public EnumPropertyModelConfiguration IsRequired()
        {
            IsOptional = false;
            return this;
        }
    }
}
