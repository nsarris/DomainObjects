using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public class AggregatePropertyModelConfiguration : PropertyModelConfiguration
    {
        public AggregatePropertyModelConfiguration(PropertyInfo property) : base(property)
        {
        }

        public AggregatePropertyModelConfiguration IsRequired()
        {
            IsOptional = false;
            return this;
        }
    }
}
