using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Core
{
    public abstract class DomainObject
    {
        public object this[string property]
        {
            get
            {
                return this.GetType().GetPropertyEx(property).Get(this);
            }
        }
    }

    public abstract class AggregateRoot : DomainEntity
    {

    }

    public abstract class Aggregate : DomainEntity
    {

    }

    public class AggregateReadOnlyList<T> : TrackableReadOnlyList<T>
        where T : Aggregate
    {

    }

    public class AggregateList<T> : TrackableList<T>
        where T : Aggregate
    {

    }

    public class ValueReadOnlyList<T> : TrackableReadOnlyList<T>
    {

    }

    public class ValueList<T> : TrackableList<T>
    {

    }





}
