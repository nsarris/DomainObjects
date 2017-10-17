using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Repositories
{
    public interface IRepositoryFactory
    {
        IEntityQueryProvider<TEntity> CreateQueryProvider<TEntity>() where TEntity : Core.DomainEntity;
        IEntityQueryProvider<TEntity, TKey> CreateQueryProvider<TEntity, TKey>() where TEntity : Core.DomainEntity<TKey>;
        TQueryProvider ResolveQueryProvider<TQueryProvider>();

        IEntityCommandHandler<TEntity> CreateCommandHandler<TEntity>() where TEntity : Core.DomainEntity;
        IEntityCommandHandler<TEntity, TKey> CreateCommandHandler<TEntity, TKey>() where TEntity : Core.DomainEntity<TKey>;
        TCommandHandler ResolveCommandHandler<TCommandHandler>();

        IEntityFactory<TEntity> CreateEntityFactory<TEntity>() where TEntity : Core.DomainEntity;
        TCommandHandler ResolveEntityFactory<TCommandHandler>();


        IRepository<TEntity> CreateRepository<TEntity>() where TEntity : Core.DomainEntity;
        IRepository<TEntity, TKey> CreateRepository<TEntity, TKey>() where TEntity : Core.DomainEntity<TKey>;
        TRepository ResolveRepository<TRepository>();
    }
}
