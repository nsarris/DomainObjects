using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public class ValueTypePropertyModelConfiguration : PropertyModelConfiguration
    {
        public ValueTypePropertyModelConfiguration(PropertyInfo property) : base(property)
        {
        }
    }
}
