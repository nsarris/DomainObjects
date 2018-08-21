﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamix;
using Dynamix.Reflection;

namespace DomainObjects.Core
{
    public abstract class DomainValue : DomainObject
    {
        //private const int InitialHash = 23;
        private const int HashMultiplier = 37;

        private ObjectComparer comparer = new ObjectComparer();

        protected virtual bool GetIsShallow() => false;

        public override bool Equals(object obj)
        {
            return GetIsShallow() ? comparer.ShallowEquals(this,obj) : comparer.DeepEquals(this, obj);
        }

        public bool Equals(DomainValue obj)
        {
            return GetIsShallow() ? comparer.ShallowEquals(this, obj) : comparer.DeepEquals(this, obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var properties = this.GetType().GetPropertiesEx().ToList();

                if (properties.Count == 0)
                    return 0;

                // It's possible for two objects to return the same hash code based on 
                // identically valued properties, even if they're of two different types, 
                // so we include the object's type in the hash calculation
                int hashCode = this.GetType().GetHashCode();

                foreach (var prop in properties)
                {
                    object value = prop.Get(this);
                    if (value != null)
                        hashCode = (hashCode * HashMultiplier) ^ value.GetHashCode();
                }

                //TODO: Nest enumerables

                return hashCode;
            }
        }

        public static bool operator ==(DomainValue x, DomainValue y)
        {
            if (x is null && y is null)
                return true;

            if (x is null || y is null)
                return false;

            return x.Equals(y);
        }

        public static bool operator ==(object x, DomainValue y)
        {
            if (x is DomainValue domainValue)
                return domainValue == x;
            else
                return false;
        }

        public static bool operator ==(DomainValue x, object y)
        {
            if (y is DomainValue domainValue)
                return domainValue == x;
            else
                return false;
        }

        public static bool operator !=(DomainValue x, DomainValue y)
        {
            return !(x == y);
        }

        public static bool operator !=(object x, DomainValue y)
        {
            return !(x == y);
        }

        public static bool operator !=(DomainValue x, object y)
        {
            return !(x == y);
        }
    }
}
