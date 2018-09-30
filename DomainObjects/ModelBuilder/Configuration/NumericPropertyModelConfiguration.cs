using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public class NumericPropertyModelConfiguration : PropertyModelConfiguration
    {
        int? precision = 0;
        int? scale = 0;
        decimal? minValue;
        decimal? maxValue;

        internal NumericPropertyModelConfiguration(PropertyInfo property) : base(property)
        {
        }

        public NumericPropertyModelConfiguration HasPrecision(int precision)
        {
            this.precision = precision;
            return this;
        }

        public NumericPropertyModelConfiguration HasScale(int scale)
        {
            this.scale = scale;
            return this;
        }

        public NumericPropertyModelConfiguration HasMinValue(decimal minValue)
        {
            this.minValue = minValue;
            return this;
        }

        public NumericPropertyModelConfiguration HasMaxValue(decimal maxValue)
        {
            this.maxValue = maxValue;
            return this;
        }

        public new NumericPropertyModelConfiguration IsRequired()
        {
            IsOptional = false;
            return this;
        }
    }
}
