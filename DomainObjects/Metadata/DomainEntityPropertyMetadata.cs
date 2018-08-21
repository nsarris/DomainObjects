using DomainObjects.Core;
using Dynamix.Reflection;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Metadata
{
    public enum DomainValueType
    {
        String = 1,
        Boolean,
        Number,
        DateTime,
        TimeSpan,
        Complex,
    }

    public enum DomainPropertyType
    {
        Value = 1,
        Aggregate,
        AggregateList
    }

    public class DomainEntityPropertyMetadata
    {
        public PropertyInfoEx Property { get; }
        //public bool IsKeyMember { get; }
        //public int KeyPosition { get; }
        public Type Type => Property.Type;
        public string Name => Property.Name;
        public bool IsNullableType { get; }
        //public bool IsImmutable { get; }
        public DomainPropertyType DomainPropertyType { get; } = 0;
        public DomainValueType DomainValueType { get; } = 0;

        public DomainEntityPropertyMetadata(PropertyInfoEx property)
        {
            Property = property;
            //IsKeyMember = keyPosition.HasValue;
            //KeyPosition = keyPosition ?? -1;
            IsNullableType = property.Type.IsNullable();

            var valueType = TypeHelper.GetSupportedValueType(property.Type);
            if (valueType.HasValue)
                DomainValueType = valueType.Value;
        }
    }
}
