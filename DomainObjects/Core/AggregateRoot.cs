namespace DomainObjects.Core
{
    public abstract class AggregateRoot<T> : DomainEntity<T>
        where T : AggregateRoot<T>
    {

    }

    public abstract class AggregateRoot<T, TKey> : AggregateRoot<T>
        where T : AggregateRoot<T, TKey>
    {
        
    }
}
