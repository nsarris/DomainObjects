using System.Reflection;

namespace DomainObjects.ModelBuilder
{
    public class AggregatePropertyModelConfiguration : PropertyModelConfiguration
    {
        public AggregatePropertyModelConfiguration(PropertyInfo property) : base(property)
        {
        }
    }
}
