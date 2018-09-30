using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public class BooleanPropertyModelConfiguration : PropertyModelConfiguration
    {
        internal BooleanPropertyModelConfiguration(PropertyInfo property) : base(property)
        {
        }

        public BooleanPropertyModelConfiguration IsRequired()
        {
            IsOptional = false;
            return this;
        }
    }
}
