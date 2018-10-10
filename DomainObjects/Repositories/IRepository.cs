using DomainObjects.Core;
using DomainObjects.Metadata;
using System.Collections.Generic;
using System.Text;

namespace DomainObjects.Repositories
{
    public interface IRepository<TEntity> : IEntityQueryProvider<TEntity>, IEntityCommandHandler<TEntity>, IEntityFactory<TEntity>
        where TEntity : AggregateRoot<TEntity>
    {

    }

    public interface IRepository<TEntity, TKey> : IEntityQueryProvider<TEntity, TKey>, IEntityCommandHandler<TEntity, TKey>
        where TEntity : AggregateRoot<TEntity, TKey>
    {

    }
}