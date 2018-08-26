using DomainObjects.ChangeTracking;
using System.Collections.Generic;

namespace DomainObjects.Core
{
    public class ValueList<T> : TrackableList<T>
    {
        public ValueList()
        {
        }

        public ValueList(IList<T> list) : base(list)
        {
        }

        public ValueList(IEnumerable<T> collection) : base(collection)
        {
        }
    }
}
