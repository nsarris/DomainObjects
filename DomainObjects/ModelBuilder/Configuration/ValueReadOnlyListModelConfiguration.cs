using DomainObjects.Core;
using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public class ValueReadOnlyListPropertyModelConfiguration : PropertyModelConfiguration
    {
        public ValueReadOnlyListPropertyModelConfiguration(PropertyInfo property) : base(property)
        {
        }
    }

    public class ValueReadOnlyListEntityPropertyModelConfiguration<T> : ValueReadOnlyListPropertyModelConfiguration, IEntityPropertyModelConfiguration<T>
        where T : DomainEntity
    {
        private readonly EntityModelBuilderConfiguration<T> propertyConfiguration;

        public ValueReadOnlyListEntityPropertyModelConfiguration(EntityModelBuilderConfiguration<T> propertyConfiguration, PropertyInfo property) : base(property)
        {
            this.propertyConfiguration = propertyConfiguration;
        }
        public EntityModelBuilderConfiguration<T> End()
        {
            return propertyConfiguration;
        }
    }

    public class ValueReadOnlyListValueObjectPropertyModelConfiguration<T> : ValueReadOnlyListPropertyModelConfiguration, IValueObjectPropertyModelConfiguration<T>
        where T : DomainValueObject<T>
    {
        private readonly ValueTypeModelBuilderConfiguration<T> propertyConfiguration;

        public ValueReadOnlyListValueObjectPropertyModelConfiguration(ValueTypeModelBuilderConfiguration<T> propertyConfiguration, PropertyInfo property) : base(property)
        {
            this.propertyConfiguration = propertyConfiguration;
        }
        public ValueTypeModelBuilderConfiguration<T> End()
        {
            return propertyConfiguration;
        }
    }
}
