//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DomainObjects.Repositories
//{
//    public class EntityQueryProvider<TEntity> : IEntityQueryProvider<TEntity>
//        where TEntity : Core.DomainEntity
//    {
//        public virtual  bool SupportsQueryable => false;

//        public TEntity GetById(object id)
//        {
//            throw new NotImplementedException();
//        }

//        public IQueryable<TEntity> ToQueryable()
//        {
//            throw new NotSupportedException("IQueryable is not supported for this query provider");
//        }
//    }
//}
