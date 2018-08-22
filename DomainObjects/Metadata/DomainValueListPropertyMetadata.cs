using DomainObjects.ModelBuilder.Configuration;
using DomainObjects.ModelBuilder.Descriptors;
using System;

namespace DomainObjects.Metadata
{
    public class DomainValueListPropertyMetadata : DomainPropertyMetadata
    {
        public override DomainPropertyType DomainPropertyType => DomainPropertyType.ValueList;
        public override bool IsList => true;
        public override bool IsValueType => true;
        
        public Type ElementType { get; }
        public bool IsImmutable { get; }
        public DomainValueType ElementDomainValueType { get; }
        public Type EffectiveListElementType { get; }
        

        internal DomainValueListPropertyMetadata(ValueListPropertyDescriptor descriptor, PropertyModelConfiguration configuration) : base(descriptor)
        {
            if (descriptor.IsReadOnly)
            {
                var listConfiguration = configuration as ValueReadOnlyListModelConfiguration;
            }
            else
            {
                var listConfiguration = configuration as ValueListModelConfiguration;
            }

            ElementType = descriptor.ElementType;
            IsImmutable = descriptor.IsReadOnly;
            ElementDomainValueType = TypeHelper.GetSupportedValueType(ElementType, out var effectiveType).Value;
            EffectiveListElementType = effectiveType;

            
        }
    }
}
