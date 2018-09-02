using DomainObjects.Core;
using DomainObjects.Metadata;
using Dynamix.Reflection;
using System;

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
            else if (effectiveType.IsOrSubclassOfGenericDeep(typeof(DomainValueObject<>),out var _))
                return DomainValueType.Complex;
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

        public static bool IsOrSubclassOfGenericDeep(this Type type, Type openGenericType)
        {
            return IsOrSubclassOfGenericDeep(type, openGenericType, out var _);
        }

        public static bool IsOrSubclassOfGenericDeep(this Type type, Type openGenericType, out Type actualType)
        {
            actualType = null;
            while (type != typeof(object))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == openGenericType)
                {
                    actualType = type;
                    return true;
                }
                type = type.BaseType;
            }
            return false;
        }

        //public static bool HasGenericDefinition(this Type type, Type openGenericType, out Type actualType)
        //{
        //    actualType = null;

        //    if (type.IsGenericType && type.GetGenericTypeDefinition() == openGenericType)
        //    {
        //        actualType = type;
        //        return true;
        //    }

        //    return false;
        //}

        public static bool IsOrSubclassOfDeep(this Type type, Type typeToCompare)
        {
            if (typeToCompare == typeof(object))
                return true;

            while (type != typeof(object))
            {
                if (type == typeToCompare)

                    return true;

                type = type.BaseType;
            }
            return false;
        }

        public static bool IsSubclassOfDeep(this Type type, Type typeToCompare)
        {
            
            return type.BaseType.IsOrSubclassOfDeep(typeToCompare);
        }
    }
}
