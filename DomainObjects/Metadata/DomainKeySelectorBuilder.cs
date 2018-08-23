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
        static readonly string CLASS_NAME_POSTFIX = "_DomainEntityKey_";

        public static Func<DomainEntity, object> BuildSelector(Type type, IEnumerable<PropertyInfo> keyPropertiesEnumerable)
        {
            var keyProperties = keyPropertiesEnumerable is List<PropertyInfo> keyPropertiesList ? keyPropertiesList : keyPropertiesEnumerable.ToList();

            var parameter = Expression.Parameter(typeof(DomainEntity));

            if (!keyProperties.Any())
                keyProperties = type.GetProperties().ToList();

            Func<DomainEntity, object> selectorFunction = null;

            //if (keyProperties.Count == 1)
            //{
            //    selectorFunction = Expression.Lambda<Func<DomainEntity, object>>(Expression.Convert(
            //        Expression.Property(Expression.Convert(parameter, type), keyProperties.Single()), typeof(object))
            //        , new[] { parameter }).Compile();
            //}
            //else
            {
                var dynamicTypeBuilderDescriptor =
                    new DynamicTypeDescriptorBuilder("_" + type.FullName + CLASS_NAME_POSTFIX)
                    //TODO: This can be removed when the type knows when to go shallow
                    .HasBaseType<DomainKey>();

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

                var keyValueTypeCtor = keyValueType.GetConstructors().First(x => x.GetParameters().Length > 0);

                var valueCtorExpression = Expression.New(
                    constructor: keyValueTypeCtor,
                    arguments: keyProperties
                        .Select(x => Expression.Property(
                                Expression.Convert(parameter, type), x))
                        .ToArray()
                    );

                var keyType = typeof(DomainKeyWrapper<>).MakeGenericTypeCached(keyValueType);
                var keyCtorSet = keyType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { keyValueType }, null);
                var keyCtorUnSet = keyType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(UnsetKey) }, null);

                var keySetCtorExpression = Expression.New(
                    constructor: keyCtorSet,
                    arguments: valueCtorExpression);

                var keyUnSetCtorExpression = Expression.New(
                    constructor: keyCtorUnSet,
                    arguments: Expression.Call(
                                Expression.Convert(parameter, type), nameof(DomainEntity.GetUnSetKey), new Type[0], new Expression[0]));

                var isKeySetExpresion = Expression.Call(
                                Expression.Convert(parameter, type), nameof(DomainEntity.GetKeyIsSet), new Type[0], new Expression[0]);

                var body = Expression.Condition(
                        test: Expression.Equal(isKeySetExpresion, ExpressionEx.Constants.True),
                        ifTrue: keySetCtorExpression,
                        ifFalse: keyUnSetCtorExpression
                    );

                selectorFunction = Expression.Lambda<Func<DomainEntity, object>>(body, parameter).Compile();
            }

            return selectorFunction;
        }
    }
}
