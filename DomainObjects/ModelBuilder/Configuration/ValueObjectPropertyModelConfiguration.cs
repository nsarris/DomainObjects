using DomainObjects.Core;
using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public class ValueObjectPropertyModelConfiguration : PropertyModelConfiguration
    {
        public ValueObjectPropertyModelConfiguration(PropertyInfo property) : base(property)
        {
        }
    }

    public class ValueObjectEntityPropertyModelConfiguration<T> : ValueObjectPropertyModelConfiguration, IEntityPropertyModelConfiguration<T>
        where T : DomainEntity
    {
        private readonly EntityModelBuilderConfiguration<T> propertyConfiguration;

        public ValueObjectEntityPropertyModelConfiguration(EntityModelBuilderConfiguration<T> propertyConfiguration, PropertyInfo property) : base(property)
        {
            this.propertyConfiguration = propertyConfiguration;
        }
        public EntityModelBuilderConfiguration<T> End()
        {
            return propertyConfiguration;
        }
    }

    public class ValueObjectValueObjectPropertyModelConfiguration<T> : ValueObjectPropertyModelConfiguration, IValueObjectPropertyModelConfiguration<T>
    where T : DomainValueObject<T>
    {
        private readonly ValueTypeModelBuilderConfiguration<T> propertyConfiguration;

        public ValueObjectValueObjectPropertyModelConfiguration(ValueTypeModelBuilderConfiguration<T> propertyConfiguration, PropertyInfo property) : base(property)
        {
            this.propertyConfiguration = propertyConfiguration;
        }
        public ValueTypeModelBuilderConfiguration<T> End()
        {
            return propertyConfiguration;
        }
    }
}
