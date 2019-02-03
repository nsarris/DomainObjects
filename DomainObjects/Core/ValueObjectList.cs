using DomainObjects.ChangeTracking;
using System.Collections.Generic;

namespace DomainObjects.Core
{
    public interface IValueObjectList<out T> : ITrackableCollection<T>, IValueObjectReadOnlyList<T>
    where T : DomainValueObject<T>
    {

    }

    public class ValueObjectList<T> : TrackableList<T>, IValueObjectList<T>
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
