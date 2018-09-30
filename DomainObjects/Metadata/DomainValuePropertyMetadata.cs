using DomainObjects.ModelBuilder;
using DomainObjects.ModelBuilder.Configuration;
using DomainObjects.ModelBuilder.Descriptors;
using System;

namespace DomainObjects.Metadata
{
    public class DomainValuePropertyMetadata : DomainPropertyMetadata
    {
        public override DomainPropertyType DomainPropertyType => DomainPropertyType.Value;
        public override bool IsList => false;
        public override bool IsValueType => true;

        public bool IsKeyMember { get; }
        public int? KeyPosition { get; }
        public DomainValueType DomainValueType { get; }
        public Type EffectiveValueType { get; }
        public Type EnumType { get; }
        internal DomainValuePropertyMetadata(ValuePropertyDescriptor descriptor, PropertyModelConfiguration configuration, int? keyPosition) : base(descriptor)
        {
            KeyPosition = keyPosition;
            IsKeyMember = keyPosition.HasValue;
            DomainValueType = TypeHelper.GetSupportedValueType(descriptor.Property.Type, out var effectiveType).Value;
            EffectiveValueType = effectiveType;

            switch (DomainValueType)
            {
                case DomainValueType.String:
                    var stringConfiguration = configuration as StringPropertyModelConfiguration;
                    break;
                case DomainValueType.Boolean:
                    var booleanConfiguration = configuration as BooleanPropertyModelConfiguration;
                    break;
                case DomainValueType.Number:
                    var numberConfiguration = configuration as NumericPropertyModelConfiguration;
                    break;
                case DomainValueType.DateTime:
                    var dateTimeConfiguration = configuration as DateTimePropertyModelConfiguration;
                    break;
                case DomainValueType.TimeSpan:
                    var timeSpanConfiguration = configuration as TimeSpanPropertyModelConfiguration;
                    break;
                case DomainValueType.Enum:
                    var enumConfiguration = configuration as EnumPropertyModelConfiguration;
                    EnumType = Nullable.GetUnderlyingType(descriptor.Property.Type) ?? descriptor.Property.Type;
                    break;
                case DomainValueType.ValueObject:
                    var valueTypeConfiguration = configuration as ValueTypePropertyModelConfiguration;
                    break;
            }
        }
    }
}
