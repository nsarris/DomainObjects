using DomainObjects.Core;
using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public class ValueListPropertyModelConfiguration : PropertyModelConfiguration
    {
        internal decimal? MinCount { get; set; }
        internal decimal? MaxCount { get; set; }
        public ValueListPropertyModelConfiguration(PropertyInfo property) : base(property)
        {
        }
    }

    public static class ValueListPropertyModelConfigurationExtensions
    {
        public static T HasMinCount<T>(this T c, decimal minValue)
            where T : ValueListPropertyModelConfiguration
        {
            c.MinCount = minValue;
            return c;
        }

        public static T HasMaxCount<T>(this T c, decimal maxValue)
            where T : AggregateListProprertyModelConfiguration
        {
            c.MaxCount = maxValue;
            return c;
        }
    }

    public class ValueListEntityPropertyModelConfiguration<T> : ValueListPropertyModelConfiguration, IEntityPropertyModelConfiguration<T>
        where T : DomainEntity
    {
        private readonly EntityModelBuilderConfiguration<T> propertyConfiguration;

        public ValueListEntityPropertyModelConfiguration(EntityModelBuilderConfiguration<T> propertyConfiguration, PropertyInfo property) : base(property)
        {
            this.propertyConfiguration = propertyConfiguration;
        }
        public EntityModelBuilderConfiguration<T> End()
        {
            return propertyConfiguration;
        }
    }

    public class ValueListValueObjectPropertyModelConfiguration<T> : ValueListPropertyModelConfiguration, IValueObjectPropertyModelConfiguration<T>
    where T : DomainValueObject<T>
    {
        private readonly ValueTypeModelBuilderConfiguration<T> propertyConfiguration;

        public ValueListValueObjectPropertyModelConfiguration(ValueTypeModelBuilderConfiguration<T> propertyConfiguration, PropertyInfo property) : base(property)
        {
            this.propertyConfiguration = propertyConfiguration;
        }
        public ValueTypeModelBuilderConfiguration<T> End()
        {
            return propertyConfiguration;
        }
    }
}
