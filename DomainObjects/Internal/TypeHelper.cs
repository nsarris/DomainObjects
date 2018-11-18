using DomainObjects.Core;
using DomainObjects.Metadata;
using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainObjects
{
    internal static class TypeHelper
    {
        public static DomainValueType? GetSupportedValueType(this Type type)
        {
            return GetSupportedValueType(type, out var _);
        }

        public static DomainValueType? GetSupportedValueType(this Type type, out Type effectiveType)
        {
            effectiveType = type;

            effectiveType = Nullable.GetUnderlyingType(effectiveType) ?? effectiveType;

            if (effectiveType.IsEnum)
            {
                effectiveType = Enum.GetUnderlyingType(effectiveType);
                return DomainValueType.Enum;
            }
            else if (effectiveType.Is<string>())
                return DomainValueType.String;
            else if (effectiveType.Is<bool>())
                return DomainValueType.Boolean;
            else if (effectiveType.Is<DateTime>())
                return DomainValueType.DateTime;
            else if (effectiveType.Is<TimeSpan>())
                return DomainValueType.TimeSpan;
            else if (effectiveType.IsNumeric())
                return DomainValueType.Number;
            else if (effectiveType.IsDomainValueObject())
                return DomainValueType.ValueObject;
            else
                return null;
        }

        public static bool IsSupportedValueType(this Type type)
        {
            return GetSupportedValueType(type) != null;
        }

        public static bool IsSupportedValueType(this Type type, out DomainValueType? valueType)
        {
            valueType = GetSupportedValueType(type);
            return valueType != null;
        }

        public static bool IsAggregate(this Type type)
        {
            return type.IsOrSubclassOfGeneric(typeof(Aggregate<,,>));
        }

        public static bool IsAggregateRoot(this Type type)
        {
            return type.IsOrSubclassOfGeneric(typeof(AggregateRoot<,>));
        }

        public static bool IsDomainEntity(this Type type)
        {
            return type.IsAggregate() || type.IsAggregateRoot();
        }

        public static bool IsDomainValueObject(this Type type)
        {
            return type.IsSubclassOfGeneric(typeof(DomainValueObject<>));
        }

        public static bool IsAggregateList(this Type type)
        {
            return type.ImplementsGeneric(typeof(IAggregateReadOnlyList<>));
        }

        public static bool IsAggregateList(this Type type, out Type elementType, out bool readOnly)
        {
            if (type.ImplementsGeneric(typeof(IAggregateReadOnlyList<>), out var actualType)
                || type.IsOrSubclassOfGeneric(typeof(IAggregateReadOnlyList<>), out actualType))
            {
                elementType = actualType.GetGenericArguments().Single();
                readOnly = !type.ImplementsGeneric(typeof(IAggregateList<>)) && !type.IsOrSubclassOfGeneric(typeof(IAggregateList<>));
                return true;
            }
            else
            {
                elementType = null;
                readOnly = false;
                return false;
            }
        }

        public static bool IsDomainValueObjectList(this Type type, out Type elementType, out bool readOnly)
        {
            readOnly = type.IsOrSubclassOfGeneric(typeof(ValueObjectReadOnlyList<>), out var actualType);

            if (readOnly || type.IsOrSubclassOfGeneric(typeof(ValueObjectList<>), out actualType))
            {
                elementType = actualType.GetGenericArguments().Single();
                return true;
            }
            else
            {
                elementType = null;
                return false;
            }
        }
        public static bool IsDomainObjectFactory(this Type type)
        {
            return type.IsGenericOrGenericSuperclassOf(typeof(DomainEntityFactory<,>));
        }


        public static bool IsFrameworkType(this Type type)
        {
            return type.IsGenericOrGenericSuperclassOf(typeof(AggregateRoot<,>))
                || type.IsGenericOrGenericSuperclassOf(typeof(Aggregate<,,>))
                || type.IsGenericOrGenericSuperclassOf(typeof(DomainValueObject<>));
        }

        public static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            while (type != typeof(object))
            {
                type = type.BaseType;
                yield return type;
            }
        }
    }
}
