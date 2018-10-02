using DomainObjects.Core;
using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public class AggregateReadOnlyListModelConfiguration : PropertyModelConfiguration
    {
        public AggregateReadOnlyListModelConfiguration(PropertyInfo property) : base(property)
        {
        }
    }

    public class AggregateReadOnlyListEntityPropertyModelConfiguration<T> : ValueReadOnlyListPropertyModelConfiguration, IEntityPropertyModelConfiguration<T>
        where T : DomainEntity
    {
        private readonly EntityModelBuilderConfiguration<T> propertyConfiguration;

        public AggregateReadOnlyListEntityPropertyModelConfiguration(EntityModelBuilderConfiguration<T> propertyConfiguration, PropertyInfo property) : base(property)
        {
            this.propertyConfiguration = propertyConfiguration;
        }
        public EntityModelBuilderConfiguration<T> End()
        {
            return propertyConfiguration;
        }
    }
}
