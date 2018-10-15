using DomainObjects.Core;
using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public class AggregateListProprertyModelConfiguration : PropertyModelConfiguration
    {
        internal decimal? MinCount { get; set; }
        internal decimal? MaxCount { get; set; }
        public AggregateListProprertyModelConfiguration(PropertyInfo property) : base(property)
        {
        }
    }

    public static class AggregateListPropertyModelConfigurationExtensions
    {
        public static T HasMinCount<T>(this T c, decimal minValue)
            where T : AggregateListProprertyModelConfiguration
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

    public class AggregateListEntityPropertyModelConfiguration<T> : ValueReadOnlyListPropertyModelConfiguration, IEntityPropertyModelConfiguration<T>
        where T : DomainEntity
    {
        private readonly EntityModelBuilderConfiguration<T> propertyConfiguration;

        public AggregateListEntityPropertyModelConfiguration(EntityModelBuilderConfiguration<T> propertyConfiguration, PropertyInfo property) : base(property)
        {
            this.propertyConfiguration = propertyConfiguration;
        }
        public EntityModelBuilderConfiguration<T> End()
        {
            return propertyConfiguration;
        }
    }
}
