using System.Reflection;

namespace DomainObjects.ModelBuilder
{
    public class ValueTypePropertyModelConfiguration : PropertyModelConfiguration
    {
        public ValueTypePropertyModelConfiguration(PropertyInfo property) : base(property)
        {
        }
    }
}
