using DomainObjects.Core;
using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public class BooleanPropertyModelConfiguration : PropertyModelConfiguration
    {
        internal BooleanPropertyModelConfiguration(PropertyInfo property) : base(property)
        {
        }
    }

    public static class BooleanPropertyModelConfigurationExtensions
    {
        
    }

    public class BooleanEntityPropertyModelConfiguration<T> : BooleanPropertyModelConfiguration, IEntityPropertyModelConfiguration<T>
        where T : DomainEntity
    {
        private readonly EntityModelBuilderConfiguration<T> propertyConfiguration;

        public BooleanEntityPropertyModelConfiguration(EntityModelBuilderConfiguration<T> propertyConfiguration, PropertyInfo property) : base(property)
        {
            this.propertyConfiguration = propertyConfiguration;
        }
        public EntityModelBuilderConfiguration<T> End()
        {
            return propertyConfiguration;
        }
    }

    public class BooleanValueObjectPropertyModelConfiguration<T> : BooleanPropertyModelConfiguration, IValueObjectPropertyModelConfiguration<T>
        where T : DomainValueObject<T>
    {
        private readonly ValueTypeModelBuilderConfiguration<T> propertyConfiguration;

        public BooleanValueObjectPropertyModelConfiguration(ValueTypeModelBuilderConfiguration<T> propertyConfiguration, PropertyInfo property) : base(property)
        {
            this.propertyConfiguration = propertyConfiguration;
        }
        public ValueTypeModelBuilderConfiguration<T> End()
        {
            return propertyConfiguration;
        }
    }
}
