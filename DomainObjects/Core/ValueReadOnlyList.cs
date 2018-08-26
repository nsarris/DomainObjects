using DomainObjects.ChangeTracking;
using System.Collections.Generic;

namespace DomainObjects.Core
{
    public class ValueReadOnlyList<T> : TrackableReadOnlyList<T>
    {
        public ValueReadOnlyList()
        {
        }

        public ValueReadOnlyList(IList<T> list) : base(list)
        {
        }

        public ValueReadOnlyList(IEnumerable<T> collection) : base(collection)
        {
        }
    }
}
