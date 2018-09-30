using DomainObjects.ModelBuilder.Configuration;
using System;
using System.Reflection;

namespace DomainObjects.ModelBuilder
{
    public class TimeSpanPropertyModelConfiguration : PropertyModelConfiguration
    {
        TimeSpan? minValue;
        TimeSpan? maxValue;

        internal TimeSpanPropertyModelConfiguration(PropertyInfo property) : base(property)
        {
        }

        public TimeSpanPropertyModelConfiguration HasMinValue(TimeSpan minValue)
        {
            this.minValue = minValue;
            return this;
        }

        public TimeSpanPropertyModelConfiguration HasMaxValue(TimeSpan maxValue)
        {
            this.maxValue = maxValue;
            return this;
        }

        public new TimeSpanPropertyModelConfiguration IsRequired()
        {
            IsOptional = false;
            return this;
        }
    }
}
