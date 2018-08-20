using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Core
{
    public abstract class DomainObject //: DynamicType
    {
    }

    public abstract class AggregateRoot : DomainEntity
    {

    }

    public abstract class Aggregate : DomainEntity
    {

    }

    public class AggregateReadOnlyList<T> : TrackableReadOnlyList<T>
        where T : Aggregate
    {

    }

    public class AggregateList<T> : TrackableList<T>
        where T : Aggregate
    {

    }

    public class ValueReadOnlyList<T> : TrackableReadOnlyList<T>
    {

    }

    public class ValueList<T> : TrackableList<T>
    {

    }

    //public struct DomainNumber //: DomainValue
    //{
    //    //event
    //    decimal? number;
    //    public bool IsSet { get; private set; }
    //    public bool IsNull => !number.HasValue;
    //    public bool HasValue => IsSet && !IsNull;

    //    public DomainNumber(decimal number)
    //        : this()
    //    {
    //        this.number = number;
    //        IsSet = true;
    //    }

    //    public void UnSet()
    //    {
    //        number = default(decimal);
    //        IsSet = false;
    //    }

    //    public void SetNull()
    //    {
    //        number = null;
    //        IsSet = true;
    //    }

    //    public decimal Value
    //    {
    //        get
    //        {
    //            return number ?? 0;
    //        }
    //        set
    //        {
    //            number = value;
    //            IsSet = true;
    //        }
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        if (obj is null)
    //            return false;

    //        if (obj is DomainNumber other)
    //        {
    //            if (this.HasValue != other.HasValue)
    //                return false;

    //            return this.Value == other.Value;
    //        }
    //        else
    //            return false;
    //    }

    //    public override int GetHashCode()
    //    {
    //        if (!HasValue)
    //            return 0;
    //        else
    //            return number.GetHashCode();
    //    }
    //    //implicit conversions
    //}

    public class DomainEntityPropertyDescriptor
    {
        public PropertyInfoEx Property { get; }
        public bool IsKeyMember { get; }
        public int KeyPosition { get; }
        public Type Type => Property.Type;
        public string Name => Property.Name;
        public bool IsNullableType { get; }
        //public bool IsImmutable { get; }
        public DomainPropertyTypeEnum DomainPropertyType { get; } = 0;
        public DomainValueTypeEnum DomainValueType { get; } = 0;

        public DomainEntityPropertyDescriptor(PropertyInfoEx property, int? keyPosition)
        {
            Property = property;
            IsKeyMember = keyPosition.HasValue;
            KeyPosition = keyPosition ?? -1;
            IsNullableType = property.Type.IsNullable();

            var valueType = TypeHelper.GetSupportedValueType(property.Type);
            if (valueType != null)
                DomainValueType = valueType.Value;
        }
    }

    //public class DomainEntityValuePropertyDescriptor : DomainEntityPropertyDescriptor
    //{

    //}

    //public class DomainEntityAggregatePropertyDescriptor : DomainEntityPropertyDescriptor
    //{

    //}

    //public class DomainEntityAggregatePropertyDescriptor : DomainEntityPropertyDescriptor
    //{

    //}

    public enum DomainValueTypeEnum
    {
        String = 1,
        Boolean,
        Number,
        DateTime,
        TimeSpan,
        Complex,
    }

    public enum DomainPropertyTypeEnum
    {
        Value = 1,
        Aggregate,
        AggregateList
    }

    public class DomainEntityDescriptor
    {
        private readonly Dictionary<string, DomainEntityPropertyDescriptor> propertyDescriptors
            = new Dictionary<string, DomainEntityPropertyDescriptor>();

        public Type EntityType { get; }
        public bool IsRoot { get; }

        private void GetProperties()
        {
            foreach (var property in EntityType.GetPropertiesEx())
            {
                //if not marked ignore

                
                if (property.PropertyInfo.PropertyType.IsAssignableTo(typeof(DomainKey)))
                {
                    //Key value type
                    //propertyDescriptors.Add(property.Name, new DomainEntityPropertyDescriptor(property));
                }
                if (property.PropertyInfo.PropertyType.IsAssignableTo(typeof(DomainValue)))
                {
                    //ValueTypeProperties
                }
                if (property.PropertyInfo.PropertyType.IsAssignableTo(typeof(Aggregate)))
                {
                    //AggregateProperty
                    //Visit Type
                }
                else if (property.PropertyInfo.PropertyType.IsAssignableToGenericType(typeof(AggregateList<>))
                    || property.PropertyInfo.PropertyType.IsAssignableToGenericType(typeof(AggregateReadOnlyList<>)))
                {
                    //AggregateListProperty
                    //Visit Type
                }
                else if (property.PropertyInfo.PropertyType.IsAssignableToGenericType(typeof(ValueList<>))
                    || property.PropertyInfo.PropertyType.IsAssignableToGenericType(typeof(ValueReadOnlyList<>)))
                {
                    //check if T is supportedType
                    //if complexType visit inner
                }
                //Check supported primitive types
                else if (TypeHelper.IsSupportedValueType(property.PropertyInfo.PropertyType, out var valueType))
                {

                }

                //else throw not supported / dont throw set error for validation
            }
        }

        private void ValidateModel() //SanityCheck
        {
            //At least one key member
            //Only one complex key member
            //No unsupported types
        }
    }

    class TypeHelper
    {
        public static DomainValueTypeEnum? GetSupportedValueType(Type type)
        {
            if (type.Is<string>())
                return DomainValueTypeEnum.String;
            else if (type.IsOrNullable<bool>())
                return DomainValueTypeEnum.Boolean;
            else if (type.IsOrNullable<DateTime>())
                return DomainValueTypeEnum.DateTime;
            else if (type.IsOrNullable<TimeSpan>())
                return DomainValueTypeEnum.TimeSpan;
            else if (type.IsNumericOrNullable())
                return DomainValueTypeEnum.Number;
            else
                return null;
        }

        public static bool IsSupportedValueType(Type type)
        {
            return GetSupportedValueType(type) != null;
        }

        public static bool IsSupportedValueType(Type type, out DomainValueTypeEnum? valueType)
        {
            valueType = GetSupportedValueType(type);
            return valueType != null;
        }
    }
}
