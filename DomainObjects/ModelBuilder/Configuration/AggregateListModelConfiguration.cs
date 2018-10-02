using DomainObjects.Core;
using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public class AggregateListProprertyModelConfiguration : PropertyModelConfiguration
    {
        public AggregateListProprertyModelConfiguration(PropertyInfo property) : base(property)
        {
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
