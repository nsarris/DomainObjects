using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dynamix;
using System.Reflection;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections;
using Dynamix.Reflection;

namespace DomainObjects.Core
{

    public abstract class Immutable
    {
        protected Immutable(Array data)
        {
            var i = 0;

            var props = GetType().GetPropertiesEx().ToList();
            if (data.Length != props.Count)
                throw new Exception("Contrstructor parameter count doesn't match Immutable property count");

            foreach (var prop in props)
            {
                var v = data.GetValue(i++);
                var propType = prop.PropertyInfo.PropertyType;

                if (!propType.IsPrimitiveLike())
                {
                    if (propType.IsSubclassOf(typeof(Immutable)))
                    {
                        if (v == null)
                            throw new Exception("A nested immutable cannot be null, please provide a value");
                    }
                    else // List
                    {
                        var collectionType = GetCollectionType(propType);
                        if (collectionType.TypeEnum == SupportedCollectionTypeEnum.None || !collectionType.ElementType.IsSubclassOf(typeof(Immutable)))
                            throw new Exception("Only Immutables, ReadOnlyCollection<>, ImmutableList<> and ImmutableArray<> class types are supported as Immutable properties");


                        if (v == null)
                            v = collectionType.Create();
                        else
                        {
                            if (propType != v.GetType())
                            {
                                var elementType = ImplementsIEnumerableOfImmutableType(propType);
                                if (elementType == null)
                                    throw new Exception("Unexpected collection type");

                                v = collectionType.FromIEnumerable(v);
                            }
                        }
                    }
                }

                prop.Set(this, v);
            }
        }

        public Immutable()
        {
            //CheckType(this.GetType());
            //foreach (var prop in GetType().GetPropertiesEx())
            //{
            //    var propType = prop.PropertyInfo.PropertyType;
            //    if (propType.IsArray)
            //    {
            //        prop.Set(this, Activator.CreateInstance(propType, new object[] { 0 }), true);
            //    }
            //    else if (propType.IsClass)
            //    {
            //        if (!propType.IsSubclassOf(typeof(Immutable))) // List
            //            prop.Set(this, Activator.CreateInstance(propType), true);
            //    }
            //}
        }

        private enum SupportedCollectionTypeEnum
        {
            None,
            ReadOnlyCollection,
            ImmutableArray,
            ImmutableList
        }

        private class SupportedCollectionType
        {
            public SupportedCollectionTypeEnum TypeEnum { get; set; }
            public Type ElementType { get; set; }
            public Type GetCollectionGenericType()
            {
                if (TypeEnum == SupportedCollectionTypeEnum.ReadOnlyCollection)
                    return typeof(ReadOnlyCollection<>);
                else if (TypeEnum == SupportedCollectionTypeEnum.ImmutableList)
                    return typeof(ImmutableList<>);
                else if (TypeEnum == SupportedCollectionTypeEnum.ImmutableArray)
                    return typeof(ImmutableArray<>);
                else
                    return null;
            }
            public Type GetCollectionType()
            {
                var t = GetCollectionGenericType();
                if (t == null)
                    return null;

                return t.MakeGenericType(new Type[] { ElementType });
            }

            public object Create()
            {
                if (TypeEnum == SupportedCollectionTypeEnum.ReadOnlyCollection)
                {
                    var l = Activator.CreateInstance(typeof(List<>).MakeGenericType(new[] { ElementType }));
                    return Activator.CreateInstance(GetCollectionType(), new object[] { l });
                }
                else if (TypeEnum == SupportedCollectionTypeEnum.ImmutableList)
                    return GetCollectionType().GetField("Empty").GetValue(null);
                else if (TypeEnum == SupportedCollectionTypeEnum.ImmutableArray)
                    return Activator.CreateInstance(GetCollectionType());
                else
                    return null;

            }

            public object FromIEnumerable(object enumerable)
            {
                if (TypeEnum == SupportedCollectionTypeEnum.ReadOnlyCollection)
                {
                    var list = typeof(Enumerable).GetMethod("ToList").MakeGenericMethod(new[] { ElementType }).Invoke(null, new[] { enumerable });
                    return Activator.CreateInstance(GetCollectionType(), new object[] { list });
                }
                else if (TypeEnum == SupportedCollectionTypeEnum.ImmutableList)
                    return typeof(ImmutableList).GetMethod("ToImmutableList").MakeGenericMethod(new[] { ElementType }).Invoke(null, new[] { enumerable });
                else if (TypeEnum == SupportedCollectionTypeEnum.ImmutableArray)
                    return typeof(ImmutableArray).GetMethod("ToImmutableArray").MakeGenericMethod(new[] { ElementType }).Invoke(null, new[] { enumerable });
                else
                    return null;
            }
        }

        private static SupportedCollectionType GetCollectionType(Type t)
        {
            SupportedCollectionTypeEnum tenum = SupportedCollectionTypeEnum.None;
            Type e = null;

            if ((t.IsClass || t.IsStruct()) && t.IsGenericType)
            {
                if (t.GetGenericTypeDefinition() == typeof(ReadOnlyCollection<>))
                {
                    tenum = SupportedCollectionTypeEnum.ReadOnlyCollection;
                }
                else if (t.GetGenericTypeDefinition() == typeof(ImmutableList<>))
                {
                    tenum = SupportedCollectionTypeEnum.ImmutableList;
                }
                else if (t.GetGenericTypeDefinition() == typeof(ImmutableArray<>))
                {
                    tenum = SupportedCollectionTypeEnum.ImmutableArray;
                }
                e = t.GetGenericArguments().First();
            }

            return new SupportedCollectionType { TypeEnum = tenum, ElementType = e };
        }

        private static Type ImplementsIEnumerableOfImmutableType(Type t)
        {
            foreach (var iface in t.GetInterfaces())
                if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    var typearg = iface.GetGenericArguments()[0];
                    if (typearg.IsSubclassOf(typeof(Immutable)))
                        return typearg;
                }
            return null;
        }

        public static void CheckType(Type T)
        //where T : Immutable
        {


            if (T.GetFields().Any(x => x.IsPublic))
                throw new Exception("Public fields not alloed in Immutable types, use properties with private setters");

            var propTypes = new List<Type>();
            var props = T.GetPropertiesEx().ToList();
            foreach (var prop in props)
            {
                if (prop.PublicSet)
                    throw new Exception("Public setters not allowed on Immutable types");

                var type = prop.PropertyInfo.PropertyType;
                propTypes.Add(type);
                if (!type.IsPrimitiveLike())
                {
                    if (type.IsInterface)
                    {
                        throw new Exception("Interfaces are not allowed as properties to Immutable objects, use Immutable objects.");
                    }
                    else if (type.IsClass)
                    {
                        if (!type.IsSubclassOf(typeof(Immutable)))
                        {
                            var collectionType = GetCollectionType(type);
                            if (collectionType.TypeEnum == SupportedCollectionTypeEnum.None)
                                throw new Exception("Only Immutables, ReadOnlyCollection<>, ImmutableList<> and ImmutableArray<> class types are supported as Immutable properties");
                        }
                    }
                    else if (prop.PropertyInfo.PropertyType.IsStruct())
                    {
                        throw new Exception("Structs are not allowed as properties to Immutable objects, use Immutable objects.");
                    }
                }
            }

            var foundCtor = false;
            foreach (var c in T.GetConstructors())
            {
                var cparams = c.GetParameters();
                if (cparams.Length != props.Count)
                    break;
                for (var i = 0; i < cparams.Length; i++)
                {

                    if (cparams[i].ParameterType != props[i].PropertyInfo.PropertyType)
                    {
                        var collectionType = GetCollectionType(props[i].PropertyInfo.PropertyType);
                        var isSupportedCollection = collectionType.TypeEnum != SupportedCollectionTypeEnum.None && collectionType.ElementType.IsSubclassOf(typeof(Immutable));
                        if (isSupportedCollection && ImplementsIEnumerableOfImmutableType(cparams[i].ParameterType) != collectionType.ElementType)
                            break;
                    }
                }
                foundCtor = true;
                break;
            }
            if (!foundCtor)
                throw new Exception("Immutable doesn't have a constructor with all property types, or types are not in the appropriate order.");
        }

        //public T Clone()
        //{
        //    return null;
        //}


    }

    

    public static class TypeExtensions
    {
        public static readonly Type[] PrimitiveTypes = { typeof(Int32), typeof(Boolean), typeof(Byte), typeof(Int64), typeof(Int16), typeof(Double), typeof(Single), typeof(UInt16), typeof(UInt32), typeof(UInt64), typeof(SByte), typeof(Char), typeof(IntPtr), typeof(UIntPtr), };
        public static readonly Type[] PrimitiveLikeTypes = { typeof(string), typeof(DateTime), typeof(Decimal), typeof(Guid), typeof(TimeSpan) };
        public static bool IsPrimitiveLike(this Type t)
        {
            return t.IsEnum || PrimitiveTypes.Contains(t) || PrimitiveLikeTypes.Contains(t);
        }

        public static bool IsStruct(this Type t)
        {
            return t.IsValueType && !t.IsEnum;
        }



    }



    internal class Mutation
    {
        public PropertyInfo Property { get; set; }
        public LambdaExpression Expression { get; set; }
        public object Value { get; set; }
    }

    public class Mutator<T>
        where T : Immutable
    {
        List<Mutation> mutations = new List<Mutation>();
        T source;
        public Mutator(T source)
        {
            this.source = source;
        }

        public Mutator<T> Set<TProp>(Expression<Func<T, TProp>> PropertyExpession, TProp Value)
        {
            mutations.Add(new Mutation
            {
                Property = ReflectionHelper.GetProperty(PropertyExpession),
                Expression = PropertyExpession,
                Value = Value
            });
            return this;
        }

        #region Inner Collection Add 

        public Mutator<T> Add<TProp>(Expression<Func<T, ReadOnlyCollection<TProp>>> PropertyExpession, TProp Value)
        {
            return this;
        }

        public Mutator<T> Add<TProp>(Expression<Func<T, ImmutableArray<TProp>>> PropertyExpession, TProp Value)
        {
            return this;
        }

        public Mutator<T> Add<TProp>(Expression<Func<T, IImmutableList<TProp>>> PropertyExpession, TProp Value)
        {
            return this;
        }

        #endregion

        #region Inner Collection Remove 

        public Mutator<T> Remove<TProp>(Expression<Func<T, ReadOnlyCollection<TProp>>> PropertyExpession, TProp Value)
        {
            return this;
        }

        public Mutator<T> Remove<TProp>(Expression<Func<T, ImmutableArray<TProp>>> PropertyExpession, TProp Value)
        {
            return this;
        }

        public Mutator<T> Remove<TProp>(Expression<Func<T, IImmutableList<TProp>>> PropertyExpession, TProp Value)
        {
            return this;
        }

        public Mutator<T> Remove<TProp>(Expression<Func<T, ReadOnlyCollection<TProp>>> PropertyExpession, int index)
        {
            return this;
        }

        public Mutator<T> Remove<TProp>(Expression<Func<T, ImmutableArray<TProp>>> PropertyExpession, int index)
        {
            return this;
        }

        public Mutator<T> Remove<TProp>(Expression<Func<T, IImmutableList<TProp>>> PropertyExpession, int index)
        {
            return this;
        }

        #endregion

        #region Inner Collection Clear 

        public Mutator<T> Clear<TProp>(Expression<Func<T, ReadOnlyCollection<TProp>>> PropertyExpession)
        {
            return this;
        }

        public Mutator<T> Clear<TProp>(Expression<Func<T, ImmutableArray<TProp>>> PropertyExpession)
        {
            return this;
        }

        public Mutator<T> Clear<TProp>(Expression<Func<T, IImmutableList<TProp>>> PropertyExpession)
        {
            return this;
        }

        #endregion  

        public T ToImmutable()
        {
            var ctorProps = new List<object>();
            foreach (var p in source.GetType().GetPropertiesEx())
            {
                var changedProperty = mutations.Where(x => x.Property == p.PropertyInfo).FirstOrDefault();
                if (changedProperty == null)
                    ctorProps.Add(p.Get(source));
                else
                    ctorProps.Add(changedProperty.Value);
            }

            return (T)Activator.CreateInstance(typeof(T), ctorProps.ToArray());
        }


    }
}
