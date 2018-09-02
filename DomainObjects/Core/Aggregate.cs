namespace DomainObjects.Core
{
    public abstract class Aggregate<T> : DomainEntity<T>
        where T : Aggregate<T>
    {

    }

    public abstract class Aggregate<T, TKey> : Aggregate<T>
        where T : Aggregate<T, TKey>
    {

    }

    public abstract class Aggregate<T, TParent, TKey> : Aggregate<T, TKey>
        where T : Aggregate<T, TParent, TKey>
    {
        public abstract TParent Parent { get; }
    }
}
