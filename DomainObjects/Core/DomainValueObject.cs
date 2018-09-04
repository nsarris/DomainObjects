using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DomainObjects.Internal;
using Dynamix;
using Dynamix.Reflection;

namespace DomainObjects.Core
{
    public abstract class DomainValueObject<T> : DomainObject, IEquatable<T>
        where T : DomainValueObject<T>
    {
        private const int HashMultiplier = 37;

        //TODO: Implement an activator (mutator like) to set initial properties selectively?
        
        //TODO: infer this from model metadata
        protected virtual bool GetIsShallow() => false;

        protected DomainValueObject()
        {

        }

        protected DomainValueObject(SerializationInfo info, StreamingContext context)
            :base(info, context)
        {
            
        }

        public override bool Equals(object obj)
        {
            if (obj is null || obj.GetType() != this.GetType())
                return false;

            return GetIsShallow() ? ObjectComparer.Default.ShallowEquals(this,obj) : ObjectComparer.Default.DeepEquals(this, obj);
        }

        public virtual bool Equals(T other)
        {
            return Equals(other as object);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var properties = this.GetType().GetPropertiesEx().ToList();

                if (properties.Count == 0)
                    return 0;

                // It's possible for two objects to return the same hash code based on 
                // identically valued properties, even if they are of two different types, 
                // so we include the object's type in the hash calculation
                int hashCode = this.GetType().GetHashCode();

                foreach (var prop in properties)
                {
                    object value = prop.Get(this);
                    if (value != null)
                        hashCode = (hashCode * HashMultiplier) ^ value.GetHashCode();
                }

                //TODO: Nest enumerables (immutable only)

                return hashCode;
            }
        }

        //TODO: Type sanity check (supported properties - all private setters to support mutator)

        public static bool operator ==(DomainValueObject<T> x, DomainValueObject<T> y)
        {
            if (x is null && y is null)
                return true;

            if (x is null || y is null)
                return false;

            return x.Equals(y);
        }

        public static bool operator ==(T x, DomainValueObject<T> y)
        {
            if (x is null && y is null)
                return true;

            if (x is null || y is null)
                return false;

            return y.Equals(x);
        }

        public static bool operator ==(DomainValueObject<T> x, T y)
        {
            return y == x;
        }

        public static bool operator !=(DomainValueObject<T> x, DomainValueObject<T> y)
        {
            return !(x == y);
        }

        public static bool operator !=(T x, DomainValueObject<T> y)
        {
            return !(x == y);
        }

        public static bool operator !=(DomainValueObject<T> x, T y)
        {
            return !(x == y);
        }
    }
}
