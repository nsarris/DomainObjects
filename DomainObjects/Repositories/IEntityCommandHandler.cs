using DomainObjects.Core;

namespace DomainObjects.Repositories
{
    public interface IEntityCommandHandler<TEntity>
        where TEntity : AggregateRoot<TEntity>
    {
        void Save(TEntity entity);
        void Delete(TEntity entity);
        void DeleteById(object id);
    }

    public interface IEntityCommandHandler<TEntity, in TKey> : IEntityCommandHandler<TEntity>
        where TEntity : AggregateRoot<TEntity, TKey>
    {
        void DeleteById(TKey id);
    }
}