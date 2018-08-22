﻿using Dynamix.Reflection;
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
            set
            {
                var prop = this.GetType().GetPropertyEx(property, BindingFlagsEx.AllInstance);
                if (prop.PublicSet)
                    prop.Set(value);
                else
                    throw new InvalidOperationException($"Property {prop.Name} of {this.GetType().Name} does not have a public set method");
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
        public AggregateReadOnlyList()
        {
        }

        public AggregateReadOnlyList(IList<T> list) : base(list)
        {
        }

        public AggregateReadOnlyList(IEnumerable<T> collection) : base(collection)
        {
        }
    }

    public class AggregateList<T> : TrackableList<T>
        where T : Aggregate
    {
        public AggregateList()
        {
        }

        public AggregateList(IList<T> list) : base(list)
        {
        }

        public AggregateList(IEnumerable<T> collection) : base(collection)
        {
        }
    }

    public class ValueReadOnlyList<T> : TrackableReadOnlyList<T>
    {
        public ValueReadOnlyList()
        {
        }

        public ValueReadOnlyList(IList<T> list) : base(list)
        {
        }

        public ValueReadOnlyList(IEnumerable<T> collection) : base(collection)
        {
        }
    }

    public class ValueList<T> : TrackableList<T>
    {
        public ValueList()
        {
        }

        public ValueList(IList<T> list) : base(list)
        {
        }

        public ValueList(IEnumerable<T> collection) : base(collection)
        {
        }
    }





}
