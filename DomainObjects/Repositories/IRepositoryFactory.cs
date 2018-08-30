//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DomainObjects.Repositories
//{
//    public interface IRepositoryFactory
//    {
//        //IEntityQueryProvider<TEntity> CreateQueryProvider<TEntity>() where TEntity : Core.AggregateRoot<TEntity>;
//        IEntityQueryProvider<TEntity, TKey> CreateQueryProvider<TEntity, TKey>() where TEntity : Core.AggregateRoot<TEntity, TKey>;
//        //TQueryProvider ResolveQueryProvider<TQueryProvider>();

//        //IEntityCommandHandler<TEntity> CreateCommandHandler<TEntity>() where TEntity : Core.AggregateRoot<TEntity>;
//        IEntityCommandHandler<TEntity, TKey> CreateCommandHandler<TEntity, TKey>() where TEntity : Core.AggregateRoot<TEntity, TKey>;
//        //TCommandHandler ResolveCommandHandler<TCommandHandler>();

//        IEntityFactory<TEntity, TKey> CreateEntityFactory<TEntity, TKey>() where TEntity : Core.AggregateRoot<TEntity, TKey>;
//        TCommandHandler ResolveEntityFactory<TCommandHandler>();


//        //IRepository<TEntity> CreateRepository<TEntity>() where TEntity : Core.AggregateRoot<TEntity>;
//        IRepository<TEntity, TKey> CreateRepository<TEntity, TKey>() where TEntity : Core.AggregateRoot<TEntity, TKey>;
//        //TRepository ResolveRepository<TRepository>();
//    }
//}
