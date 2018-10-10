using DomainObjects.Core;
using Dynamix;
using Dynamix.Expressions;
using Dynamix.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Metadata
{
    internal static class DomainKeySelectorBuilder
    {
        private class DummyEntity : DomainEntity<DummyEntity> { }

        static readonly string CLASS_NAME_SUFFIX = "_DomainEntityKey_";

        private static Expression BuildKeyValueSelectorExpression(Type entityType, List<PropertyInfo> keyProperties, out ParameterExpression entityParameterExpression, out Type keyValueType)
        {
            entityParameterExpression = Expression.Parameter(typeof(object));
            var convertedEntityTypeExpression = ExpressionEx.ConvertIfNeeded(entityParameterExpression, entityType);

            if (!keyProperties.Any())
                keyProperties = entityType.GetProperties().ToList();

            if (keyProperties.Count == 1)
            {
                var property = keyProperties.Single();
                keyValueType = property.PropertyType;
                return Expression.Property(convertedEntityTypeExpression, property);
            }
            else
            {
                keyValueType = BuildKeyValueType(entityType, keyProperties);

                var keyValueTypeCtor = keyValueType.GetConstructors().First(x => x.GetParameters().Length > 0);

                return Expression.New(
                    constructor: keyValueTypeCtor,
                    arguments: keyProperties
                        .Select(x => Expression.Property(convertedEntityTypeExpression, x))
                        .ToArray()
                    );
            }
        }

        public static Func<object, object> BuildKeyValueSelector(Type type, IEnumerable<PropertyInfo> keyPropertiesEnumerable)
        {
            var keyProperties = keyPropertiesEnumerable is List<PropertyInfo> keyPropertiesList ? keyPropertiesList : keyPropertiesEnumerable.ToList();

            var selector = BuildKeyValueSelectorExpression(type, keyProperties, out var parameter, out var _);
            var body = ExpressionEx.ConvertIfNeeded(selector, typeof(object));

            return Expression.Lambda<Func<object, object>>(body, parameter).Compile();
        }

        private static Type BuildKeyValueType(Type entityType, List<PropertyInfo> keyProperties)
        {
            var dynamicTypeBuilderDescriptor =
                new DynamicTypeDescriptorBuilder("_" + entityType.FullName + CLASS_NAME_SUFFIX)
                .HasBaseType<DomainKeyValue>();

            keyProperties
                .ForEach(x =>
                {
                    dynamicTypeBuilderDescriptor.AddProperty(
                        x.Name,
                        x.PropertyType,
                        config => config
                            .HasSetter(GetSetAccessModifier.None)
                            .IsInitializedInConstructor()
                   );
                });

            var keyValueType = DynamicTypeBuilder.Instance.CreateType(dynamicTypeBuilderDescriptor);
            return keyValueType;
        }

        private static Expression BuildSetKeyConstructorExpression(Type keyType, Type keyValueType, Expression keyValueSelectorExpression)
        {
            var keyCtorSet = keyType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { keyValueType }, null);

            return Expression.New(
                constructor: keyCtorSet,
                arguments: ExpressionEx.ConvertIfNeeded(keyValueSelectorExpression, keyValueType));
        }

        private static Expression BuildUnSetKeyConstructorExpression(Type keyType, Expression entityExpression)
        {
            var keyCtorUnSet = keyType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(UnassignedKey) }, null);

            return Expression.New(
                    constructor: keyCtorUnSet,
                    arguments: Expression.Call(entityExpression, nameof(DummyEntity.GetUnAssignedKey), new Type[0], new Expression[0]));
        }

        public static Func<object, object> BuildKeySelector(Type entityType, IEnumerable<PropertyInfo> keyPropertiesEnumerable)
        {
            var keyProperties = keyPropertiesEnumerable is List<PropertyInfo> keyPropertiesList ? keyPropertiesList : keyPropertiesEnumerable.ToList();
            var valueSelectorExpression = BuildKeyValueSelectorExpression(entityType, keyProperties, out var entityParameterExpression, out var keyValueType);

            var keyType = typeof(DomainKey<>).MakeGenericTypeCached(keyValueType);
            var entityExpression = ExpressionEx.ConvertIfNeeded(entityParameterExpression, entityType);

            var keySetCtorExpression = BuildSetKeyConstructorExpression(keyType, keyValueType, valueSelectorExpression);
            var keyUnSetCtorExpression = BuildUnSetKeyConstructorExpression(keyType, entityExpression);

            var isKeySetExpresion = Expression.Call(entityExpression, nameof(DummyEntity.GetKeyIsAssigned), new Type[0], new Expression[0]);

            var body = Expression.Condition(
                    test: Expression.Equal(isKeySetExpresion, ExpressionEx.Constants.True),
                    ifTrue: keySetCtorExpression,
                    ifFalse: keyUnSetCtorExpression
                );

            return Expression.Lambda<Func<object, object>>(ExpressionEx.ConvertIfNeeded(body, typeof(object)), entityParameterExpression).Compile();
        }
    }
}
