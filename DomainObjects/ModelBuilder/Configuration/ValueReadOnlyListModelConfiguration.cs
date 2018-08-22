using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public class ValueReadOnlyListModelConfiguration : PropertyModelConfiguration
    {
        public ValueReadOnlyListModelConfiguration(PropertyInfo property) : base(property)
        {
        }
    }
}
