using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Patterns
{
    public interface IBuildSpecification<T>
        //where TEntity : Core.DomainEntity
    {
        void ApplyTo(T entity);
    }

    //public class BuildSpecificationCollection<T> : IEnumerable<IBuildSpecification<T>>
    //{
    //    List<IBuildSpecification<T>> specifications;
    //    public IEnumerator<IBuildSpecification<T>> GetEnumerator()
    //    {
    //        return specifications.GetEnumerator();
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return specifications.GetEnumerator();
    //    }

    //    public BuildSpecificationCollection<T> Append
    //}
}
