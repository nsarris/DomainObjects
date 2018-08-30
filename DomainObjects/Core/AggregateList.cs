using DomainObjects.ChangeTracking;
using System.Collections.Generic;

namespace DomainObjects.Core
{
    public class AggregateList<T> : TrackableList<T>
        where T : Aggregate
    {
        public AggregateList()
        {
        }

        public AggregateList(IList<T> list) : base(list)
        {
        }

        public AggregateList(IEnumerable<T> collection) : base(collection)
        {
        }
    }
}
