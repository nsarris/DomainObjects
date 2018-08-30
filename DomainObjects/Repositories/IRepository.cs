using DomainObjects.Patterns;
using Dynamix.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Repositories
{
    //public interface IRepository<T>
    //{
    //    T GetById(object id);
    //    void Insert(T obj);
    //}

    public interface IEntityQueryProvider
    {
        bool SupportsAsync { get; }
        bool SupportsQueryable { get; }
    }

    public interface IEntityQueryProvider<TEntity> : IEntityQueryProvider
        where TEntity : Core.AggregateRoot<TEntity>
    {
        TEntity GetById(object id);
        Task<TEntity> GetByIdAsync(object id);
        IQueryable<TEntity> ToQueryable();
        SingleQueryable<TEntity> QueryById(object id);
    }

    public interface IEntityQueryProvider<TEntity, TKey> : IEntityQueryProvider
        where TEntity : Core.AggregateRoot<TEntity, TKey>
    {
        TEntity GetById(TKey id);
        Task<TEntity> GetByIdAsync(TKey id);
        SingleQueryable<TEntity> QueryById(TKey id);
    }

    public interface IEntityCommandHandler<TEntity>
        where TEntity : Core.AggregateRoot<TEntity>
    {
        void Save(TEntity entity);
        void Delete(TEntity entity);
        void DeleteById(object id);
    }

    public interface IEntityCommandHandler<TEntity, in TKey>
        where TEntity : Core.AggregateRoot<TEntity, TKey>
    {
        void Save(TEntity entity);
        void Delete(TEntity entity);
        void DeleteById(TKey id);
    }

    public interface IEntityFactory<TEntity>
        where TEntity : Core.AggregateRoot<TEntity>
    {
        TEntity Create(params IBuildSpecification<TEntity>[] buildSpecifications);
    }


    public interface IRepository<TEntity> : IEntityQueryProvider, IEntityCommandHandler<TEntity>, IEntityFactory<TEntity>
        where TEntity : Core.AggregateRoot<TEntity>
    {

    }

    public interface IRepository<TEntity, TKey> : IEntityQueryProvider<TEntity, TKey>, IEntityCommandHandler<TEntity, TKey>
        where TEntity : Core.AggregateRoot<TEntity, TKey>
    {

    }

    

    
}