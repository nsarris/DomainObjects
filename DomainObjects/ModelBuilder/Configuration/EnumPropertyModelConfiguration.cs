using DomainObjects.Core;
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
    }

    public class EnumEntityPropertyModelConfiguration<T> : EnumPropertyModelConfiguration, IEntityPropertyModelConfiguration<T>
        where T : DomainEntity
    {
        private readonly EntityModelBuilderConfiguration<T> propertyConfiguration;

        public EnumEntityPropertyModelConfiguration(EntityModelBuilderConfiguration<T> propertyConfiguration, PropertyInfo property) : base(property)
        {
            this.propertyConfiguration = propertyConfiguration;
        }
        public EntityModelBuilderConfiguration<T> End()
        {
            return propertyConfiguration;
        }
    }

    public class EnumValueObjectPropertyModelConfiguration<T> : EnumPropertyModelConfiguration, IValueObjectPropertyModelConfiguration<T>
        where T : DomainValueObject<T>
    {
        private readonly ValueTypeModelBuilderConfiguration<T> propertyConfiguration;

        public EnumValueObjectPropertyModelConfiguration(ValueTypeModelBuilderConfiguration<T> propertyConfiguration, PropertyInfo property) : base(property)
        {
            this.propertyConfiguration = propertyConfiguration;
        }
        public ValueTypeModelBuilderConfiguration<T> End()
        {
            return propertyConfiguration;
        }
    }
}
