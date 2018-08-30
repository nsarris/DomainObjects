namespace DomainObjects.Core
{
    public abstract class Aggregate : DomainEntity
    {

    }

    public interface IObjectWithParent<T>
    {

    }

    public abstract class Aggregate<TKey> : Aggregate
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

    public abstract class Aggregate<T, TKey> : Aggregate<TKey>
        where T : Aggregate<T, TKey>
    {

    }

    public abstract class Aggregate<T, TParent, TKey> : Aggregate<T, TKey>, IObjectWithParent<TParent>
        where T : Aggregate<T, TParent, TKey>
    {
        
    }
}
