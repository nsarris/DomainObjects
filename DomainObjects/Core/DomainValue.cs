using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamix;
using Dynamix.Reflection;

namespace DomainObjects.Core
{
    public class DomainValue : DomainObject
    {
        private const int HashMultiplier = 37;
        private ObjectComparer comparer = new ObjectComparer();
        public override bool Equals(object obj)
        {
            return comparer.DeepEquals(this, obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var properties = this.GetType().GetPropertiesEx().ToList();

                if (properties.Count == 0)
                {
                    return base.GetHashCode();
                }

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

                return hashCode;
            }
        }

        public static bool operator ==(DomainValue x, DomainValue y)
        {
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
                return false;

            return x.Equals(y);
        }

        public static bool operator ==(object x, DomainValue y)
        {
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
                return false;

            return y.Equals(x);
        }

        public static bool operator ==(DomainValue x, object y)
        {
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
                return false;

            return x.Equals(y);
        }

        public static bool operator !=(DomainValue x, DomainValue y)
        {
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
                return true;

            return !x.Equals(y);
        }

        public static bool operator !=(object x, DomainValue y)
        {
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
                return true;

            return !x.Equals(y);
        }

        public static bool operator !=(DomainValue x, object y)
        {
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
                return true;

            return !x.Equals(y);
        }
    }
}
