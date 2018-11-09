using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dynamix;

namespace DomainObjects.Core
{
    internal class UnassignedKey { }

    #pragma warning disable S4035 // Classes implementing "IEquatable<T>" should be sealed
    public class DomainKey : IEquatable<DomainKey>
    {
        public object Value { get; }
        public bool IsAssigned => UnassignedKey == null;
        private UnassignedKey UnassignedKey { get; }

        internal DomainKey(UnassignedKey unassignedKey)
        {
            this.UnassignedKey = unassignedKey;
        }

        internal DomainKey(object value)
        {
            this.Value = value;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DomainKey other))
                return false;

            return Equals(other);
        }

        public bool Equals(DomainKey other)
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

        public override int GetHashCode()
        {
            return IsAssigned ? Value.GetHashCode() : UnassignedKey.GetHashCode();
        }

        public static bool operator ==(DomainKey x, DomainKey y)
        {
            if (x is null && y is null)
                return true;

            if (x is null || y is null)
                return false;

            return x.Equals(y);
        }

        public static bool operator !=(DomainKey x, DomainKey y)
        {
            return !(x == y);
        }


        public static bool operator ==(object x, DomainKey y)
        {
            if (x is DomainKey domainKey)
                return domainKey == x;
            else
                return false;
        }

        public static bool operator ==(DomainKey x, object y)
        {
            return y == x;
        }

        public static bool operator !=(object x, DomainKey y)
        {
            return !(x == y);
        }

        public static bool operator !=(DomainKey x, object y)
        {
            return !(x == y);
        }
    }
    #pragma warning restore S4035 // Classes implementing "IEquatable<T>" should be sealed

    public sealed class DomainKey<T> : DomainKey, IEquatable<DomainKey<T>>, IEquatable<T>
    {
        public new T Value => (T)base.Value;
        
        internal DomainKey(T value)
            :base(value)
        {
            
        }
        internal DomainKey(UnassignedKey unassignedKey)
            :base(unassignedKey)
        {
            
        }


        public override bool Equals(object obj)
        {
            if (!(obj is DomainKey<T> other))
                return false;

            return Equals(other);
        }

        public bool Equals(T other)
        {
            return other == this;
        }

        public bool Equals(DomainKey<T> other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
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
}
