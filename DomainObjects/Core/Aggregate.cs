using System.Runtime.Serialization;

namespace DomainObjects.Core
{
    public abstract class Aggregate<T> : DomainEntity<T>
        where T : Aggregate<T>
    {
        protected Aggregate()
        {

        }
        protected Aggregate(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }

    public abstract class Aggregate<T, TKey> : Aggregate<T>
        where T : Aggregate<T, TKey>
    {
        protected Aggregate()
        {

        }
        protected Aggregate(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }

    public abstract class Aggregate<T, TParent, TKey> : Aggregate<T, TKey>
        where T : Aggregate<T, TParent, TKey>
    {
        public TParent Parent { get; }

        protected Aggregate(TParent parent)
        {
            Parent = parent;
        }
        protected Aggregate(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}
