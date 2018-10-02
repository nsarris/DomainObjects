using DomainObjects.Core;
using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public class AggregatePropertyModelConfiguration : PropertyModelConfiguration
    {
        public AggregatePropertyModelConfiguration(PropertyInfo property) : base(property)
        {
        }
    }

    public class AggregateEntityPropertyModelConfiguration<T> : ValueReadOnlyListPropertyModelConfiguration, IEntityPropertyModelConfiguration<T>
        where T : DomainEntity
    {
        private readonly EntityModelBuilderConfiguration<T> propertyConfiguration;

        public AggregateEntityPropertyModelConfiguration(EntityModelBuilderConfiguration<T> propertyConfiguration, PropertyInfo property) : base(property)
        {
            this.propertyConfiguration = propertyConfiguration;
        }
        public EntityModelBuilderConfiguration<T> End()
        {
            return propertyConfiguration;
        }
    }
}
