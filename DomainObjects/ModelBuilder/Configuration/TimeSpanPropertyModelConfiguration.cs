using DomainObjects.Core;
using DomainObjects.ModelBuilder.Configuration;
using System;
using System.Reflection;

namespace DomainObjects.ModelBuilder
{
    public class TimeSpanPropertyModelConfiguration : PropertyModelConfiguration
    {
        internal TimeSpan? MinValue { get; set; }
        internal TimeSpan? MaxValue { get; set; }

        internal TimeSpanPropertyModelConfiguration(PropertyInfo property) : base(property)
        {
        }
    }

    public static class TimeSpanPropertyModelConfigurationExtensions
    {
        public static T HasMinValue<T>(this T c, TimeSpan minValue)
            where T : TimeSpanPropertyModelConfiguration
        {
            c.MinValue = minValue;
            return c;
        }

        public static T HasMaxValue<T>(this T c, TimeSpan maxValue)
            where T : TimeSpanPropertyModelConfiguration
        {
            c.MaxValue = maxValue;
            return c;
        }
    }

    public class TimeSpanEntityPropertyModelConfiguration<T> : TimeSpanPropertyModelConfiguration, IEntityPropertyModelConfiguration<T>
        where T : DomainEntity
    {
        private readonly EntityModelBuilderConfiguration<T> propertyConfiguration;

        public TimeSpanEntityPropertyModelConfiguration(EntityModelBuilderConfiguration<T> propertyConfiguration, PropertyInfo property) : base(property)
        {
            this.propertyConfiguration = propertyConfiguration;
        }
        public EntityModelBuilderConfiguration<T> End()
        {
            return propertyConfiguration;
        }
    }

    //public class TimeSpanEntityPropertyModelConfiguration<T> : TimeSpanPropertyModelConfiguration
    //    where T : DomainEntity
    //{
    //    private readonly EntityModelBuilderConfiguration<T> propertyConfiguration;

    //    public TimeSpanEntityPropertyModelConfiguration(EntityModelBuilderConfiguration<T> propertyConfiguration, PropertyInfo property) : base(property)
    //    {
    //        this.propertyConfiguration = propertyConfiguration;
    //    }
    //    public EntityModelBuilderConfiguration<T> End()
    //    {
    //        return propertyConfiguration;
    //    }
    //}
}
