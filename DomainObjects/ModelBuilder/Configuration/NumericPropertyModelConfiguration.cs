using DomainObjects.Core;
using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public class NumericPropertyModelConfiguration : PropertyModelConfiguration
    {
        internal byte? Precision { get; set; }
        internal byte? Scale { get; set; }
        internal decimal? MinValue { get; set; }
        internal decimal? MaxValue { get; set; }

        internal NumericPropertyModelConfiguration(PropertyInfo property) : base(property)
        {
        }
    }

    public static class NumericPropertyModelConfigurationExtensions
    {
        public static T HasPrecision<T>(this T c, byte precision)
            where T : NumericPropertyModelConfiguration
        {
            c.Precision = precision;
            return c;
        }

        public static T HasScale<T>(this T c, byte scale)
            where T : NumericPropertyModelConfiguration
        {
            c.Scale = scale;
            return c;
        }

        public static T HasMinValue<T>(this T c, decimal minValue)
            where T : NumericPropertyModelConfiguration
        {
            c.MinValue = minValue;
            return c;
        }

        public static T HasMaxValue<T>(this T c, decimal maxValue)
            where T : NumericPropertyModelConfiguration
        {
            c.MaxValue = maxValue;
            return c;
        }
    }


    public class NumericEntityPropertyModelConfiguration<T> : NumericPropertyModelConfiguration, IEntityPropertyModelConfiguration<T>
        where T : DomainEntity
    {
        private readonly EntityModelBuilderConfiguration<T> propertyConfiguration;

        public NumericEntityPropertyModelConfiguration(EntityModelBuilderConfiguration<T> propertyConfiguration, PropertyInfo property) : base(property)
        {
            this.propertyConfiguration = propertyConfiguration;
        }
        public EntityModelBuilderConfiguration<T> End()
        {
            return propertyConfiguration;
        }
    }

    //public class NumericEntityPropertyModelConfiguration<T> : NumericPropertyModelConfiguration
    //    where T : DomainEntity
    //{
    //    private readonly EntityModelBuilderConfiguration<T> propertyConfiguration;

    //    public NumericEntityPropertyModelConfiguration(EntityModelBuilderConfiguration<T> propertyConfiguration, PropertyInfo property) : base(property)
    //    {
    //        this.propertyConfiguration = propertyConfiguration;
    //    }
    //    public EntityModelBuilderConfiguration<T> End()
    //    {
    //        return propertyConfiguration;
    //    }
    //}
}
