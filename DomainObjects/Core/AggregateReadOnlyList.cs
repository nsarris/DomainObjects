using DomainObjects.ChangeTracking;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DomainObjects.Core
{
    public interface IAggregateReadOnlyList<out T> : IReadOnlyList<T>
        where T : Aggregate<T>
    {

    }

    public class AggregateReadOnlyList<T> : ReadOnlyCollection<T>, IAggregateReadOnlyList<T>
        where T : Aggregate<T>
    {
        public AggregateReadOnlyList(IList<T> list) : base(list.ToList())
        {
        }

        public AggregateReadOnlyList(IEnumerable<T> collection) : base(collection.ToList())
        {
        }
    }
}
