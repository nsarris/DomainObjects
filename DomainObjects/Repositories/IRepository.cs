using DomainObjects.Core;
using DomainObjects.Metadata;
using DomainObjects.Patterns;
using Dynamix.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    public interface IEntityFactory<TEntity>
        where TEntity : AggregateRoot<TEntity>
    {
        TEntity Create(params IBuildSpecification<TEntity>[] buildSpecifications);
    }



    public interface IRepository<TEntity> : IEntityQueryProvider<TEntity>, IEntityCommandHandler<TEntity>, IEntityFactory<TEntity>
        where TEntity : AggregateRoot<TEntity>
    {

    }

    public interface IRepository<TEntity, TKey> : IEntityQueryProvider<TEntity, TKey>, IEntityCommandHandler<TEntity, TKey>
        where TEntity : AggregateRoot<TEntity, TKey>
    {

    }
}