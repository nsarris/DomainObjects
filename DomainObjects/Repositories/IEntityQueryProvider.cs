using DomainObjects.Core;
using Dynamix.QueryableExtensions;
using System.Linq;
using System.Threading.Tasks;

namespace DomainObjects.Repositories
{
    public interface IEntityQueryProvider<TEntity>
        where TEntity : AggregateRoot<TEntity>
    {
        bool SupportsAsync { get; }
        bool SupportsQueryable { get; }
        TEntity GetById(object id);
        Task<TEntity> GetByIdAsync(object id);
        IQueryable<TEntity> ToQueryable();
        SingleQueryable<TEntity> QueryById(object id);
    }

    public interface IEntityQueryProvider<TEntity, TKey> : IEntityQueryProvider<TEntity>
        where TEntity : AggregateRoot<TEntity, TKey>
    {
        TEntity GetById(TKey id);
        Task<TEntity> GetByIdAsync(TKey id);
        SingleQueryable<TEntity> QueryById(TKey id);
    }
}