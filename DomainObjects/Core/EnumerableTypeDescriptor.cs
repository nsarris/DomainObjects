using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Core
{
    //public class EnumerableTypeDescriptor
    //{
    //    private static readonly Type[] enumerableInterfaces = new Type[] { typeof(IEnumerable), typeof(IEnumerable<>) };

    //    private static readonly Type[] arrayInterfaces = new Type[] { };
    //    private static readonly Type[] arrayInterfacesRO = new Type[] { };
    //    private static readonly Type[] arrayTypesExtra = new Type[] { typeof(Array) };

    //    private static readonly Type[] collectionInterfaces = new[] { typeof(ICollection), typeof(ICollection<>) };
    //    private static readonly Type[] collectionInterfacesRO = new[] { typeof(IReadOnlyCollection<>) };
    //    private static readonly Type[] collectionTypesExtra = new Type[] { };

    //    private static readonly Type[] listInterfaces = new[] { typeof(IList), typeof(IList<>) };
    //    private static readonly Type[] listInterfacesRO = new[] { typeof(IReadOnlyList<>) };
    //    private static readonly Type[] listTypesExtra = new Type[] { };

    //    private static readonly Type[] dicInterfaces = new[] { typeof(IDictionary), typeof(IDictionary<,>) };
    //    private static readonly Type[] dicInterfacesRO = new[] { typeof(IReadOnlyDictionary<,>) };
    //    private static readonly Type[] dictTypesExtra = new Type[] { };

    //    private static readonly HashSet<Type> allTypes = new HashSet<Type>();
    //    private static readonly HashSet<Type> allIterfaces = new HashSet<Type>();

    //    public bool IsGeneric { get; private set; }
    //    public Type Type { get; private set; }
    //    public Type GenericType { get; private set; }
    //    //public IEnumerable<Type> GenericTypeArguments { get; private set; }
    //    public bool IsIndexed { get; private set; }
    //    public bool IsDictionary { get; private set; }
    //    public bool IsArray { get; private set; }
    //    public bool IsCollection { get; private set; }
    //    public bool IsList { get; private set; }
    //    public Type DictionaryKeyType { get; private set; }
    //    public Type DictionaryElementType { get; private set; }
    //    public Type ElementType { get; private set; }
    //    public bool IsReadOnly { get; private set; }
    //    private MethodInfo enumeratorMethod;
    //    static EnumerableTypeDescriptor()
    //    {
    //        foreach (var t in enumerableInterfaces) allIterfaces.Add(t);
    //        foreach (var t in arrayInterfaces) allIterfaces.Add(t);
    //        foreach (var t in arrayInterfacesRO) allIterfaces.Add(t);
    //        foreach (var t in arrayTypesExtra) allTypes.Add(t);
    //        foreach (var t in collectionInterfaces) allIterfaces.Add(t);
    //        foreach (var t in collectionInterfacesRO) allIterfaces.Add(t);
    //        foreach (var t in collectionTypesExtra) allTypes.Add(t);
    //        foreach (var t in listInterfaces) allIterfaces.Add(t);
    //        foreach (var t in listInterfacesRO) allIterfaces.Add(t);
    //        foreach (var t in listTypesExtra) allTypes.Add(t);
    //        foreach (var t in dicInterfaces) allIterfaces.Add(t);
    //        foreach (var t in dicInterfacesRO) allIterfaces.Add(t);
    //        foreach (var t in dictTypesExtra) allIterfaces.Add(t);
    //    }

    //    public EnumerableTypeDescriptor(Type t)
    //    {
    //        if (t.IsInterface)
    //            throw new Exception("EnumerableTypeDescriptor only works on classes");

    //        var interfaces = t.GetInterfaces();
    //        var enumerable = interfaces.Any(x => x == typeof(IEnumerable));
    //        var genericEnumerables = interfaces.Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)).ToList();

    //        if (!enumerable)
    //            throw new Exception("Type is not an enumerable type");

    //        this.Type = t;
    //        this.ElementType = typeof(object);

    //        if (t.IsArray)
    //        {
    //            this.IsArray = true;
    //            this.IsGeneric = false;
    //            this.ElementType = t.GetElementType();
    //            return;
    //        }
            
    //        var enumeratorMethods = t.GetMethods().Where(x => x.Name == "GetEnumerator" && x.GetParameters().Count() == 0).ToList();
    //        //These should never happen in theory
    //        if (enumeratorMethods.Count == 0)
    //            throw new Exception("Unexpected error, GetEnumerator method was not found");
    //        if (enumeratorMethods.Count > 1)
    //            throw new Exception("Unexpected error, multiple GetEnumerator methods found");

    //        enumeratorMethod = enumeratorMethods.FirstOrDefault();
    //        var enumeratorType = enumeratorMethod.ReturnType;
    //        var genericEnumerator = enumeratorType.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerator<>)).FirstOrDefault();

    //        if (genericEnumerator != null)
    //        {
    //            this.IsGeneric = true;
    //            this.ElementType = genericEnumerator.GetGenericArguments()[0];
    //            this.GenericType = t.GetGenericTypeDefinition();
    //            interfaces = interfaces.Where(x => x.IsGenericType && x.GetGenericArguments()[0] == this.ElementType).ToArray();
    //        }
    //        else
    //            interfaces = interfaces.Where(x => !x.IsGenericType).ToArray();

    //        var arrayInterface = interfaces.Where(x => arrayInterfaces.Contains(x)).FirstOrDefault();
    //        var arrayInterfaceRO = interfaces.Where(x => arrayInterfacesRO.Contains(x)).FirstOrDefault();
    //        var arrayType = arrayTypesExtra.Where(x => x == t).FirstOrDefault();

    //        if (arrayInterface != null || arrayInterfaceRO != null || arrayType != null)
    //        {
    //            this.IsArray = true;
    //            this.IsReadOnly = arrayInterface == null && arrayType == null;
    //        }

    //        var listInterface = interfaces.Where(x => listInterfaces.Contains(x)).FirstOrDefault();
    //        var listInterfaceRO = interfaces.Where(x => listInterfacesRO.Contains(x)).FirstOrDefault();
    //        var listType = listTypesExtra.Where(x => x == t).FirstOrDefault();

    //        if (listInterface != null || listInterfaceRO != null || listType != null)
    //        {
    //            this.IsList = true;
    //            this.IsReadOnly = listInterface == null && listType == null;
    //        }

    //        var collectionIterface = interfaces.Where(x => collectionInterfaces.Contains(x)).FirstOrDefault();
    //        var collectionIterfaceRO = interfaces.Where(x => collectionInterfacesRO.Contains(x)).FirstOrDefault();
    //        var collectionType = collectionTypesExtra.Where(x => x == t).FirstOrDefault();

    //        if (collectionIterface != null || collectionIterfaceRO != null || collectionType != null)
    //        {
    //            this.IsCollection = true;
    //            this.IsReadOnly = (collectionIterface == null && collectionType == null);
    //        }

    //        if (enumeratorType.GetInterfaces().Any(x => x == typeof(IDictionaryEnumerator)))
    //        {
    //            this.IsDictionary = true;
    //            this.DictionaryElementType = typeof(object);
    //            this.DictionaryKeyType = typeof(object);
    //            this.IsGeneric = genericEnumerator != null;
    //            if (this.IsGeneric)
    //            {
    //                var genericEnumeratorParameters = genericEnumerator.GetGenericArguments();
    //                if (genericEnumeratorParameters.Length == 1
    //                    && genericEnumeratorParameters[0].IsGenericType
    //                    && genericEnumeratorParameters[0].GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
    //                {
    //                    var kvpTypeArgs = genericEnumeratorParameters[0].GetGenericArguments();
    //                    this.DictionaryKeyType = kvpTypeArgs[0];
    //                    this.DictionaryElementType = kvpTypeArgs[1];
    //                }
    //            }


    //            var dictionaryTypes = t.GetInterfaces().Where(x =>
    //                (dicInterfaces.Contains(x) || dicInterfacesRO.Contains(x) || dictTypesExtra.Contains(x))
    //                ||
    //                (x.IsGenericType && (dicInterfaces.Contains(x.GetGenericTypeDefinition()) || dicInterfacesRO.Contains(x.GetGenericTypeDefinition()) || dictTypesExtra.Contains(x.GetGenericTypeDefinition())))
    //                )
    //                .ToList();

    //            if (dictionaryTypes.Count > 0)
    //            {
    //                var dictionaryType =
    //                    this.IsGeneric
    //                        ?
    //                        dictionaryTypes.Where(x =>
    //                        x.IsGenericType && x.GetGenericArguments()[0] == this.DictionaryKeyType && x.GetGenericArguments()[1] == this.DictionaryElementType)
    //                        .FirstOrDefault()
    //                        :
    //                        dictionaryTypes.Where(x => !x.IsGenericType)
    //                        .FirstOrDefault();

    //                if (dictionaryType != null)
    //                    this.IsReadOnly = !dicInterfaces.Contains(dictionaryType) && !dictTypesExtra.Contains(dictionaryType);
    //            }
    //        }
    //    }

    //    public static EnumerableTypeDescriptor Get(Type t)
    //    {
    //        if (!t.GetInterfaces().Any(x => x == typeof(IEnumerable)))
    //            return null;
    //        else
    //            return new EnumerableTypeDescriptor(t);
    //    }


    //    private class EnumerableMethods
    //    {
    //        public MethodInfo ToList { get; set; }
    //        public MethodInfo ToArray { get; set; }
    //        public MethodInfo AsEnumerable { get; set; }
    //    }

    //    private static ConcurrentDictionary<Type, EnumerableMethods> toListMethods = new ConcurrentDictionary<Type, EnumerableMethods>();
        
    //    private static EnumerableMethods GetEnumerableMethods(Type elementType)
    //    {
    //        if (!toListMethods.TryGetValue(elementType,out var methods))
    //        {
    //            methods = new EnumerableMethods
    //            {
    //                ToList = typeof(Enumerable).GetMethod("ToList").MakeGenericMethod(new[] { elementType }),
    //                ToArray = typeof(Enumerable).GetMethod("ToArray").MakeGenericMethod(new[] { elementType }),
    //                AsEnumerable = typeof(Enumerable).GetMethod("AsEnumerable").MakeGenericMethod(new[] { elementType }),
    //            };
    //            toListMethods.TryAdd(elementType, methods);
    //        }
    //        return methods;
    //    }

    //    private EnumerableMethods enumerableMethods;
        
    //    private EnumerableMethods GetEnumerableMethods()
    //    {
    //        if (enumerableMethods == null)
    //            enumerableMethods = GetEnumerableMethods(ElementType);
    //        return enumerableMethods;
    //    }

    //    public IList ToList(object enumerable)
    //    {
    //        return (IList)GetEnumerableMethods(ElementType).ToList.Invoke(null, new[] { enumerable });
    //    }

    //    public List<T> ToList<T>(object enumerable)
    //    {
    //        var list = ToList(enumerable);
    //        if (typeof(T) == ElementType)
    //            return (List<T>)list;
    //        else
    //            return list.Cast<T>().ToList();
    //    }

    //    public IReadOnlyCollection<T> ToReadOnlyCollection<T>(object enumerable)
    //    {
    //        var list = ToList<T>(enumerable);
    //        return (IReadOnlyCollection<T>)Activator.CreateInstance(typeof(ReadOnlyCollection<T>), new object[] { list });
    //    }

    //    public Array ToArray(object enumerable)
    //    {
    //        return (Array)GetEnumerableMethods().ToArray.Invoke(null, new[] { enumerable });
    //    }

    //    public T[] ToArray<T>(object enumerable)
    //    {
    //        var array = ToArray(enumerable);
    //        if (typeof(T) == ElementType)
    //            return (T[])array;
    //        else
    //            return array.Cast<T>().ToArray();
    //    }

    //    public IEnumerator GetEnumerator(object enumerable)
    //    {
    //        return (IEnumerator)enumeratorMethod.Invoke(enumerable, new object[] { });
    //    }
    //    public IEnumerator<T> GetEnumerator<T>(object enumerable)
    //    {
    //        var enumerator = (IEnumerator)enumeratorMethod.Invoke(enumerable, new object[] { });
    //        if (typeof(T) == ElementType)
    //            return (IEnumerator<T>)enumerator;
    //        else
    //            throw new Exception("ElementType does not match requested enumerator type");
    //    }

    //    public IEnumerable AsEnumerable(object enumerable)
    //    {
    //        return (IEnumerable)enumerable;
    //    }

    //    public IEnumerable<T> AsEnumerable<T>(object enumerable)
    //    {
    //        return (IEnumerable<T>)GetEnumerableMethods().AsEnumerable.Invoke(null, new[] { enumerable });
    //    }
    //}
}
