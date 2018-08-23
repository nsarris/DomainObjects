using DomainObjects.Core;

namespace DomainObjects.Metadata
{
    public interface IKeyProvider
    {
        IDomainKey GetKey();
        //TODO: need keyValueSelector
        //object GetKeyValue();
    }

    public interface IKeyProvider<TKey> : IKeyProvider
    {
        new DomainKeyWrapper<TKey> GetKey();
        //new TKey GetKeyValue();
    }
}
