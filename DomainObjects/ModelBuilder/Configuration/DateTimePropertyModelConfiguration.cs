using DomainObjects.Core;
using System;
using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public class DateTimePropertyModelConfiguration : PropertyModelConfiguration
    {
        internal byte? Precision { get; set; }
        internal DateTime? MinValue { get; set; }
        internal DateTime? MaxValue { get; set; }

        internal DateTimePropertyModelConfiguration(PropertyInfo property) : base(property)
        {
        }
    }

    public static class DateTimePropertyModelConfigurationExtensions
    {
        public static T HasPrecision<T>(this T c, byte precision)
            where T : DateTimePropertyModelConfiguration
        {
            c.Precision = precision;
            return c;
        }

        public static T HasMinValue<T>(this T c, DateTime minValue)
            where T : DateTimePropertyModelConfiguration
        {
            c.MinValue = minValue;
            return c;
        }

        public static T HasMaxValue<T>(this T c, DateTime maxValue)
            where T : DateTimePropertyModelConfiguration
        {
            c.MaxValue = maxValue;
            return c;
        }
    }

    public class DateTimeEntityPropertyModelConfiguration<T> : DateTimePropertyModelConfiguration, IEntityPropertyModelConfiguration<T>
    where T : DomainEntity
    {
        private readonly EntityModelBuilderConfiguration<T> propertyConfiguration;

        public DateTimeEntityPropertyModelConfiguration(EntityModelBuilderConfiguration<T> propertyConfiguration, PropertyInfo property) : base(property)
        {
            this.propertyConfiguration = propertyConfiguration;
        }
        public EntityModelBuilderConfiguration<T> End()
        {
            return propertyConfiguration;
        }
    }

    //public class DateTimeEntityPropertyModelConfiguration<T> : DateTimePropertyModelConfiguration
    //    where T : DomainEntity
    //{
    //    private readonly EntityModelBuilderConfiguration<T> propertyConfiguration;

    //    public DateTimeEntityPropertyModelConfiguration(EntityModelBuilderConfiguration<T> propertyConfiguration, PropertyInfo property) : base(property)
    //    {
    //        this.propertyConfiguration = propertyConfiguration;
    //    }
    //    public EntityModelBuilderConfiguration<T> End()
    //    {
    //        return propertyConfiguration;
    //    }
    //}
}
