using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public class AggregateReadOnlyListModelConfiguration : PropertyModelConfiguration
        //where T : Aggregate
    {
        public AggregateReadOnlyListModelConfiguration(PropertyInfo property) : base(property)
        {
        }
    }
}
