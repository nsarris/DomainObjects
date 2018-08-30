using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamix;
using Dynamix.Reflection;

namespace DomainObjects.Core
{
    internal class UnassignedKey { }

    public interface IDomainKey
    {
        object Value { get; }
        bool IsAssigned { get; }
    }


    public sealed class DomainKey<T> : IDomainKey, IEquatable<DomainKey<T>>, IEquatable<T>
    {
        public T Value { get; }
        public bool IsAssigned => UnassignedKey == null;
        private UnassignedKey UnassignedKey { get; }

        object IDomainKey.Value => Value;

        internal DomainKey(UnassignedKey unassignedKey)
        {
            this.UnassignedKey = unassignedKey;
        }

        internal DomainKey(T value)
        {
            this.Value = value;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DomainKey<T> other))
                return false;

            return Equals(other);
        }

        public bool Equals(DomainKey<T> other)
        {
            if (other is null)
                return false;

            if (IsAssigned && other.IsAssigned)
            {
                if (Value == null && other.Value == null)
                    return true;
                if (Value == null || other.Value == null)
                    return false;

                return Value.Equals(other.Value);
            }
            else if (!IsAssigned && !other.IsAssigned)
                return other.UnassignedKey == UnassignedKey;
            else
                return false;
        }

        public bool Equals(T other)
        {
            return other == this;
        }

        public override int GetHashCode()
        {
            return IsAssigned ? Value.GetHashCode() : UnassignedKey.GetHashCode();
        }

        public static bool operator ==(DomainKey<T> x, DomainKey<T> y)
        {
            if (x is null && y is null)
                return true;

            if (x is null || y is null)
                return false;

            return x.Equals(y);
        }

        public static bool operator !=(DomainKey<T> x, DomainKey<T> y)
        {
            return !(x == y);
        }




        public static bool operator ==(object x, DomainKey<T> y)
        {
            if (x is DomainKey<T> domainKey)
                return domainKey == x;
            else
                return false;
        }

        public static bool operator ==(DomainKey<T> x, object y)
        {
            return y == x;
        }

        public static bool operator !=(object x, DomainKey<T> y)
        {
            return !(x == y);
        }

        public static bool operator !=(DomainKey<T> x, object y)
        {
            return !(x == y);
        }

        public static bool operator ==(T x, DomainKey<T> y)
        {
            if (y == null && x == null)
                return true;
            else if (y == null)
                return false;

            if (y.IsAssigned)
                return y.Value.Equals(x);
            else
                return false;
        }

        public static bool operator ==(DomainKey<T> x, T y)
        {
            return y == x;
        }

        public static bool operator !=(T x, DomainKey<T> y)
        {
            return !(x == y);
        }

        public static bool operator !=(DomainKey<T> x, T y)
        {
            return !(x == y);
        }
    }

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
