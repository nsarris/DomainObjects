using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamix;
using Dynamix.Reflection;

namespace DomainObjects.Core
{
    internal class UnsetKey { }

    public interface IDomainKey
    {
        object Value { get; }
        bool IsSet { get; }
    }


    public sealed class DomainKey<T> : IDomainKey, IEquatable<DomainKey<T>>, IEquatable<T>
    {
        public T Value { get; }
        public bool IsSet => UnsetKey == null;
        private UnsetKey UnsetKey { get; }

        object IDomainKey.Value => Value;

        internal DomainKey(UnsetKey unsetKey)
        {
            this.UnsetKey = unsetKey;
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

            if (IsSet && other.IsSet)
            {
                if (Value == null && other.Value == null)
                    return true;
                if (Value == null || other.Value == null)
                    return false;

                return Value.Equals(other.Value);
            }
            else if (!IsSet && !other.IsSet)
                return other.UnsetKey == UnsetKey;
            else
                return false;
        }

        public bool Equals(T other)
        {
            return other == this;
        }

        public override int GetHashCode()
        {
            return IsSet ? Value.GetHashCode() : UnsetKey.GetHashCode();
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

            if (y.IsSet)
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

    public class DomainKeyValue : DomainValue
    {
        protected override bool GetIsShallow()
        {
            return true;
        }
    }
}
