using System.Runtime.Serialization;

namespace DomainObjects.Core
{
    public abstract class AggregateRoot<T> : DomainEntity<T>
        where T : AggregateRoot<T>
    {
        protected AggregateRoot()
        {

        }
        protected AggregateRoot(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }

    public abstract class AggregateRoot<T, TKey> : AggregateRoot<T>
        where T : AggregateRoot<T, TKey>
    {
        protected AggregateRoot()
        {

        }
        protected AggregateRoot(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}
