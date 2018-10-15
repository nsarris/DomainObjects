using DomainObjects.ChangeTracking;
using System.Collections.Generic;

namespace DomainObjects.Core
{
    public class ValueObjectList<T> : TrackableList<T>
        where T : DomainValueObject<T>
    {
        public ValueObjectList()
        {
        }

        public ValueObjectList(IList<T> list) : base(list)
        {
        }

        public ValueObjectList(IEnumerable<T> collection) : base(collection)
        {
        }
    }
}
