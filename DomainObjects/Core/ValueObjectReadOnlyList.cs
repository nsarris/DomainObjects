using DomainObjects.ChangeTracking;
using System.Collections.Generic;

namespace DomainObjects.Core
{
    public class ValueObjectReadOnlyList<T> : TrackableReadOnlyList<T>
        where T : DomainValueObject<T>
    {
        public ValueObjectReadOnlyList()
        {
        }

        public ValueObjectReadOnlyList(IList<T> list) : base(list)
        {
        }

        public ValueObjectReadOnlyList(IEnumerable<T> collection) : base(collection)
        {
        }
    }
}
