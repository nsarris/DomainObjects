using DomainObjects.ChangeTracking;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DomainObjects.Core
{
    public interface IValueObjectReadOnlyList<out T> : IReadOnlyList<T>
        where T : DomainValueObject<T>
    {

    }

    public class ValueObjectReadOnlyList<T> : ReadOnlyCollection<T>, IValueObjectReadOnlyList<T>
        where T : DomainValueObject<T>
    {
        public ValueObjectReadOnlyList() : this(Enumerable.Empty<T>())
        {

        }

        public ValueObjectReadOnlyList(IList<T> list) : base(list?.ToList() ?? new List<T>())
        {
        }

        public ValueObjectReadOnlyList(IEnumerable<T> collection) : base(collection?.ToList() ?? new List<T>())
        {
        }
    }
}
