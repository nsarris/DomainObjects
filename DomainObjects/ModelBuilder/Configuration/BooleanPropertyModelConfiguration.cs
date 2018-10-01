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

    public class BooleanEntityPropertyModelConfiguration<T> : BooleanPropertyModelConfiguration
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

    //public class BooleanEntityPropertyModelConfiguration<T> : BooleanPropertyModelConfiguration
    //    where T : DomainEntity
    //{
    //    private readonly EntityModelBuilderConfiguration<T> propertyConfiguration;

    //    public BooleanEntityPropertyModelConfiguration(EntityModelBuilderConfiguration<T> propertyConfiguration, PropertyInfo property) : base(property)
    //    {
    //        this.propertyConfiguration = propertyConfiguration;
    //    }
    //    public EntityModelBuilderConfiguration<T> End()
    //    {
    //        return propertyConfiguration;
    //    }
    //}
}
