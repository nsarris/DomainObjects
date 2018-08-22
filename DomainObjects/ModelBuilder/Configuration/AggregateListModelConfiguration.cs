using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public class AggregateListModelConfiguration : PropertyModelConfiguration
        //where T : Aggregate
    {
        public AggregateListModelConfiguration(PropertyInfo property) : base(property)
        {
        }
    }
}
