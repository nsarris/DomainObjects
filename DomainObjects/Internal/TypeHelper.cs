using DomainObjects.Metadata;
using Dynamix.Reflection;
using System;

namespace DomainObjects
{
    static class TypeHelper
    {
        public static DomainValueType? GetSupportedValueType(this Type type)
        {
            if (type.Is<string>())
                return DomainValueType.String;
            else if (type.IsOrNullable<bool>())
                return DomainValueType.Boolean;
            else if (type.IsOrNullable<DateTime>())
                return DomainValueType.DateTime;
            else if (type.IsOrNullable<TimeSpan>())
                return DomainValueType.TimeSpan;
            else if (type.IsNumericOrNullable())
                return DomainValueType.Number;
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

        public static bool IsOrSubclassOfGenericDeep(this Type type, Type openGenericType, out Type actualType)
        {
            actualType = null;
            while (type != typeof(object))
            {
                if (type.IsGenericTypeDefinition && type.GetGenericTypeDefinition() == openGenericType)
                {
                    actualType = type;
                    return true;
                }
                type = type.BaseType;
            }
            return false;
        }

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
