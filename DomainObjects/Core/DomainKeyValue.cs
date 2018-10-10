using System;
using System.Linq;
using Dynamix.Reflection;

namespace DomainObjects.Core
{
    public class DomainKeyValue : IEquatable<DomainKeyValue>
    {
        private const int HashMultiplier = 31;

        public override bool Equals(object obj)
        {
            if (obj is null || obj.GetType() != this.GetType())
                return false;

            return Internal.ObjectComparer.Default.ShallowEquals(this, obj);
        }

        public virtual bool Equals(DomainKeyValue other)
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


        public static bool operator ==(DomainKeyValue x, DomainKeyValue y)
        {
            if (x is null && y is null)
                return true;

            if (x is null || y is null)
                return false;

            return x.Equals(y);
        }

        public static bool operator ==(object x, DomainKeyValue y)
        {
            if (x is null && y is null)
                return true;

            if (x is null || y is null)
                return false;

            return y.Equals(x);
        }

        public static bool operator ==(DomainKeyValue x, object y)
        {
            return y == x;
        }

        public static bool operator !=(DomainKeyValue x, DomainKeyValue y)
        {
            return !(x == y);
        }

        public static bool operator !=(object x, DomainKeyValue y)
        {
            return !(x == y);
        }

        public static bool operator !=(DomainKeyValue x, object y)
        {
            return !(x == y);
        }
    }
}
