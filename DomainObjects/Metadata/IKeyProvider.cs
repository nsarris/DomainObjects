using DomainObjects.Core;

namespace DomainObjects.Metadata
{
    public interface IKeyProvider
    {
        DomainKey GetKey();
        //void SetKey(object key);
        //object GetKeyValue();
        //bool KeyEquals(object other);
    }

    public interface IKeyProvider<TKey> : IKeyProvider
    {
        new DomainKey<TKey> GetKey();
        //void SetKey(TKey key);
        //new TKey GetKeyValue();
        //bool KeyEquals(DomainEntity<TKey> other);
    }
}
