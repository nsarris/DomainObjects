using DomainObjects.Core;
using Dynamix;
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

    //TODO: Move these interfaces
    public interface IKeyProvider
    {
        object GetKey();
    }

    public interface IKeyProvider<TKey> : IKeyProvider
    {
        new TKey GetKey();
    }

    internal static class DomainKeySelectorBuilder
    {
        static readonly string CLASS_NAME_PREFIX = "_DomainEntityKey_";

        public static Func<DomainEntity, object> BuildSelector(Type type, IEnumerable<PropertyInfo> keyProperties)
        {
            var parameter = Expression.Parameter(typeof(DomainEntity));

            if (!keyProperties.Any())
                keyProperties = type.GetProperties();

            Func<DomainEntity, object> d = null;

            if (keyProperties.Count() == 1)
            {
                d = Expression.Lambda<Func<DomainEntity, object>>(Expression.Convert(
                    Expression.Property(Expression.Convert(parameter, type), keyProperties.Single()), typeof(object))
                    , new[] { parameter }).Compile();
            }
            else
            {
                //TODO: Readonly properties / Immutable
                var dprops = keyProperties
                    .Select(x => new DynamicTypeProperty()
                    {
                        Name = x.Name,
                        Type = x.PropertyType
                    }).ToList();

                var t = DynamicTypeBuilder.Instance.CreateAndRegisterType(CLASS_NAME_PREFIX + type.FullName, dprops, false, typeof(DomainKey));

                var ctor = Expression.New(t);

                var convertedItem = Expression.Convert(parameter, type);
                var memberAssignments = new List<MemberAssignment>();

                foreach (var key in keyProperties)
                {
                    var targetProperty = t.GetProperty(key.Name);
                    memberAssignments.Add(Expression.Bind(targetProperty, Expression.Property(convertedItem, key)));
                }

                var initExp = Expression.MemberInit(ctor, memberAssignments);

                d = Expression.Lambda<Func<DomainEntity, object>>(initExp, parameter).Compile();
            }

            return d;
        }
    }
}
