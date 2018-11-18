using DomainObjects.ChangeTracking;
using System.Collections.Generic;

namespace DomainObjects.Core
{
    public interface IAggregateList<out T> : ITrackableCollection<T>, IAggregateReadOnlyList<T>
        where T : Aggregate<T>
    {

    }

    public class AggregateList<T> : TrackableList<T>, IAggregateList<T>
        where T : Aggregate<T>
    {
        public AggregateList()
        {
        }

        public AggregateList(IList<T> list) : base(list ?? new List<T>())
        {
        }

        public AggregateList(IEnumerable<T> collection) : base(collection ?? new List<T>())
        {
        }
    }
}
