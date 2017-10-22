using DomainObjects.Patterns;
using Dynamix.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Repositories
{
    public interface IQueryRepository<T>
    {
        T GetById(object id);
        void Insert(T obj);
    }

    public interface IEntityQueryProvider<TEntity>
        where TEntity : Core.DomainEntity
    {
        bool SupportsAsync { get; }
        TEntity GetById(object id);
        Task<TEntity> GetByIdAsync(object id);
        SingleQueryable<TEntity> QueryById(object id);
        bool SupportsQueryable { get; }
        IQueryable<TEntity> ToQueryable();
    }

    public interface IEntityQueryProvider<TEntity, TKey> : IEntityQueryProvider<TEntity>
        where TEntity : Core.DomainEntity<TKey>
    {
        TEntity GetById(TKey id);
        Task<TEntity> GetByIdAsync(TKey id);
        SingleQueryable<TEntity> QueryById(TKey id);
    }

    public interface IEntityCommandHandler<TEntity>
        where TEntity : Core.DomainEntity
    {
        void Save(TEntity entity);
        void Delete(TEntity entity);
        void DeleteById(object id);
    }

    public interface IEntityCommandHandler<TEntity, TKey> : IEntityCommandHandler<TEntity>
        where TEntity : Core.DomainEntity<TKey>
    {
        void DeleteById(TKey id);
    }

    public interface IEntityFactory<TEntity>
        where TEntity : Core.DomainEntity
    {
        TEntity Create(params IBuildSpecification<TEntity>[] buildSpecifications);
    }


    public interface IRepository<TEntity> : IEntityQueryProvider<TEntity>, IEntityCommandHandler<TEntity>, IEntityFactory<TEntity>
        where TEntity : Core.DomainEntity
    {

    }

    public interface IRepository<TEntity, TKey> : IRepository<TEntity>, IEntityQueryProvider<TEntity, TKey>, IEntityCommandHandler<TEntity, TKey>, IEntityFactory<TEntity>
        where TEntity : Core.DomainEntity<TKey>
    {

    }

    

    
}