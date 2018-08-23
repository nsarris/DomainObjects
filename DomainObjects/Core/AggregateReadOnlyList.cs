using DomainObjects.ChangeTracking;
using System.Collections.Generic;

namespace DomainObjects.Core
{
    public class AggregateReadOnlyList<T> : TrackableReadOnlyList<T>
        where T : Aggregate
    {
        public AggregateReadOnlyList()
        {
        }

        public AggregateReadOnlyList(IList<T> list) : base(list)
        {
        }

        public AggregateReadOnlyList(IEnumerable<T> collection) : base(collection)
        {
        }
    }





}
