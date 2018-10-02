using DomainObjects.Core;
using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public class UnsupportedTypePropertyModelConfiguration : PropertyModelConfiguration
    {
        public UnsupportedTypePropertyModelConfiguration(PropertyInfo property) : base(property)
        {
        }
    }

    public class UnsupportedTypeEntityPropertyModelConfiguration<T> : UnsupportedTypePropertyModelConfiguration, IEntityPropertyModelConfiguration<T>
    where T : DomainEntity
    {
        private readonly EntityModelBuilderConfiguration<T> propertyConfiguration;

        public UnsupportedTypeEntityPropertyModelConfiguration(EntityModelBuilderConfiguration<T> propertyConfiguration, PropertyInfo property) : base(property)
        {
            this.propertyConfiguration = propertyConfiguration;
        }
        public EntityModelBuilderConfiguration<T> End()
        {
            return propertyConfiguration;
        }
    }
}
