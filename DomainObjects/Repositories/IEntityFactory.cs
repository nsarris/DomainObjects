using DomainObjects.Core;
using System.Collections.Generic;

namespace DomainObjects.Repositories
{
    public interface IEntityFactory<TEntity>
        where TEntity : AggregateRoot<TEntity>
    {
        TEntity Create(params object[] parameters);
        TEntity Create(params (string name, object value)[] namedValues);
        TEntity Create(IEnumerable<object> parameters);
        TEntity Create(IEnumerable<(string name, object value)> namedValues);
    }
    public interface IEntityFactory<TEntity, in TConstructorParameters>
        where TEntity : AggregateRoot<TEntity>
    {
        TEntity Create(TConstructorParameters constructorParameters);
    }
}