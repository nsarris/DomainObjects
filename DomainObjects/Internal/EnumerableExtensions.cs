using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainObjects.Metadata
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> SelectManyRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            return !source.Any() ? source :
                source.Concat(
                    source
                    .SelectMany(i => selector(i) ?? Enumerable.Empty<T>())
                    .SelectManyRecursive(selector)
                );
        }
    }
}
