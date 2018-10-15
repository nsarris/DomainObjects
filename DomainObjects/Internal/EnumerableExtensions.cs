using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainObjects.Metadata
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> SelectManyRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector, bool excludeSource = false)
        {
            return !source.Any() ? source :
                (excludeSource ? Enumerable.Empty<T>() : source).Concat(
                    source
                    .SelectMany(i => selector(i) ?? Enumerable.Empty<T>())
                    .SelectManyRecursive(selector)
                );
        }

        public static void AddIf<T>(this IList<T> list, Func<T,bool> predicate, T item)
        {
            if (predicate(item))
                list.Add(item);
        }

        public static void AddIfNotNull<T>(this IList<T> list, T item)
        {
            list.AddIf(x =>  x != null, item);
        }
    }
}
