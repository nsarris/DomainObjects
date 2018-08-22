using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public class DateTimePropertyModelConfiguration : PropertyModelConfiguration
    {
        int? precision = 0;

        internal DateTimePropertyModelConfiguration(PropertyInfo property) : base(property)
        {
        }

        public DateTimePropertyModelConfiguration HasPrecision(int precision)
        {
            this.precision = precision;
            return this;
        }

        public new DateTimePropertyModelConfiguration IsRequired()
        {
            base.IsRequired();
            return this;
        }
    }
}
