using DomainObjects.ModelBuilder.Configuration;
using DomainObjects.ModelBuilder.Descriptors;
using System;

namespace DomainObjects.Metadata
{
    public class DomainAggregateListPropertyMetadata : DomainPropertyMetadata
    {
        public override DomainPropertyType DomainPropertyType => DomainPropertyType.AggregateList;
        public override bool IsList => true;
        public override bool IsValueType => false;

        public Type AggregateType { get; }
        public bool IsImmutable { get; }

        internal DomainAggregateListPropertyMetadata(AggregateListPropertyDescriptor descriptor, PropertyModelConfiguration configuration) : base(descriptor)
        {
            if (descriptor.IsReadOnly)
            {
                var listConfiguration = configuration as AggregateReadOnlyListModelConfiguration;
            }
            else
            {
                var listConfiguration = configuration as AggregateListProprertyModelConfiguration;
            }

            AggregateType = descriptor.ElementType;
            IsImmutable = descriptor.IsReadOnly;
        }
    }
}
