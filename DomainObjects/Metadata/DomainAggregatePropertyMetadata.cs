using DomainObjects.ModelBuilder.Configuration;
using DomainObjects.ModelBuilder.Descriptors;

namespace DomainObjects.Metadata
{
    public class DomainAggregatePropertyMetadata : DomainPropertyMetadata
    {
        public override DomainPropertyType DomainPropertyType => DomainPropertyType.Aggregate;
        public override bool IsList => false;
        public override bool IsValueType => false;
        internal DomainAggregatePropertyMetadata(AggregatePropertyDescriptor descriptor, PropertyModelConfiguration configuration) : base(descriptor)
        {
            var aggregateConfiguration = configuration as AggregatePropertyModelConfiguration;
            
        }
    }
}
