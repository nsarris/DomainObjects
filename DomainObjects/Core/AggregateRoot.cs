namespace DomainObjects.Core
{
    public abstract class AggregateRoot : DomainEntity
    {

    }
    public abstract class AggregateRoot<TKey> : AggregateRoot
    {
        public new DomainKey<TKey> GetKey()
        {
            return (DomainKey<TKey>)base.GetKey();
        }

        public void SetKey(TKey key)
        {
            base.SetKey(key);
        }

        public new TKey GetKeyValue()
        {
            return (TKey)base.GetKeyValue();
        }

        public bool KeyEquals(DomainEntity<TKey> other)
        {
            return this.GetKey() == other.GetKey();
        }
    }

    public abstract class AggregateRoot<T, TKey> : DomainEntity<TKey>
        where T : AggregateRoot<T, TKey>
    {

    }
}
