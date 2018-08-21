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

        public static Func<object, object> BuildSelector(Type type, IEnumerable<PropertyInfo> keyProperties)
        {
            var parameter = Expression.Parameter(typeof(object));

            if (keyProperties.Count() == 0)
                keyProperties = type.GetProperties();

            Func<object, object> d = null;

            if (keyProperties.Count() == 1)
            {
                d = Expression.Lambda<Func<object, object>>(Expression.Convert(
                    Expression.Property(Expression.Convert(parameter, type), keyProperties.Single()), typeof(object))
                    , new[] { parameter }).Compile();
            }
            else
            {
                //TODO: Readonly properties
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

                d = Expression.Lambda<Func<object, object>>(initExp, parameter).Compile();
            }

            return d;
        }
    }

    internal static class DomainEntityKeyProvider
    {
        static ConcurrentDictionary<Type, Func<object,object>> keySelectors = new ConcurrentDictionary<Type, Func<object, object>>();
        static readonly string CLASS_NAME_PREFIX = "_DomainEntityKey_";
        public static Func<object, object> GetKeySelector<T>()
        {
            return GetKeySelector(typeof(T));
        }

        public static Func<object, object> GetKeySelector(Type T)
        {
            if (!keySelectors.TryGetValue(T, out Func<object, object> d))
            {
                d = CreateSelector(T);
                keySelectors.TryAdd(T, d);
            }
            
            return d;
        }

        public static Func<object, object> GetKeySelector(object o)
        {
            if (o == null)
                throw new ArgumentNullException();

            return GetKeySelector(o.GetType());
        }

        public static object GetKey(object o)
        {
            var selector = GetKeySelector(o);
            return selector.Invoke(o);
        }

        private static Func<object, object> CreateSelector(Type type)
        {
            var parameter = Expression.Parameter(typeof(object));

            var keyprops = type.GetProperties()
                .Where(x => x.HasAttribute<DomainKeyAttribute>());

            if (keyprops.Count() == 0)
                keyprops = type.GetProperties().Take(1);

            Func<object, object> d = null;

            if (keyprops.Count() == 1)
            {
                d = Expression.Lambda<Func<object, object>>(Expression.Convert(
                    Expression.Property(Expression.Convert(parameter, type), keyprops.Single()),typeof(object))
                    , new[] { parameter }).Compile();
            }
            else
            {
                var dprops = keyprops
                    .Select(x => new DynamicTypeProperty()
                    {
                        Name = x.Name,
                        Type = x.PropertyType
                    }).ToList();

                var t = DynamicTypeBuilder.Instance.CreateAndRegisterType(CLASS_NAME_PREFIX + type.Name, dprops, false, typeof(DomainKey));

                var ctor = Expression.New(t);

                var convertedItem = Expression.Convert(parameter, type);
                var memberAssignments = new List<MemberAssignment>();

                foreach (var key in keyprops)
                {
                    var targetProperty = t.GetProperty(key.Name);
                    memberAssignments.Add(Expression.Bind(targetProperty, Expression.Property(convertedItem, key)));
                }

                var initExp = Expression.MemberInit(ctor, memberAssignments);

                d = Expression.Lambda<Func<object, object>>(initExp, parameter).Compile();
            }

            return d;
        }
    }

    public class DomainEntityKeyProvider<TKey> 
    {
        //Func<object, TKey> keySelector;
        //static readonly string CLASS_NAME_PREFIX = "_DomainEntityKey_";
        
        public static object GetKey(object o)
        {
            return CreateSelector(o.GetType())(o);
        }

        private static Func<object,object> CreateSelector(Type type)
        {
            var keyprops = type.GetProperties()
                .Where(x => x.HasAttribute<DomainKeyAttribute>());

            if (keyprops.Count() != 1)
                throw new Exception("TypedKeySelector can only be applied to an object with a single key property");

            var keyProp = keyprops.Single();

            var parameter = Expression.Parameter(typeof(object));
            
            //TODO return clone if not value type

            return Expression.Lambda<Func<object,object>>(
            Expression.Property(Expression.Convert(parameter,type), keyProp)
            , new[] { parameter }).Compile();
        }
    }

}
