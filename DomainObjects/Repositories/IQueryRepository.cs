using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Repositories
{
    public interface IQueryRepository<TEntity,TKey>
        where TEntity : Core.DomainEntity<TKey>
    {
        TEntity GetById(TKey id);
    }

    public interface ICommandHandler<TEntity, TKey>
        where TEntity : Core.DomainEntity<TKey>
    {
        void Save(TEntity entity);
        void Delete(TEntity entity);
        void DeleteById(TKey id);
    }

    public interface IEntityFactory<TEntity>
        where TEntity : Core.DomainEntity
    {
        TEntity Create(params IBuildSpecification<TEntity>[] buildSpecifications);
    }


    public interface IBuildSpecification<TEntity>
        where TEntity : Core.DomainEntity
    {
        void ApplyTo(TEntity entity);
    }
}
