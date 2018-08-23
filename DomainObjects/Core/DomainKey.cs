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


    public sealed class DomainKeyWrapper<T> : IDomainKey, IEquatable<DomainKeyWrapper<T>>
    {
        public T Value { get; }
        public bool IsSet => UnsetKey == null;
        private UnsetKey UnsetKey { get; }

        object IDomainKey.Value => Value;

        internal DomainKeyWrapper(UnsetKey unsetKey)
        {
            this.UnsetKey = unsetKey;
        }

        internal DomainKeyWrapper(T value)
        {
            this.Value = value;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DomainKeyWrapper<T> other))
                return false;

            return Equals(other);
        }

        public bool Equals(DomainKeyWrapper<T> other)
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

        public static bool operator ==(DomainKeyWrapper<T> x, DomainKeyWrapper<T> y)
        {
            if (x is null && y is null)
                return true;

            if (x is null || y is null)
                return false;

            return x.Equals(y);
        }

        public static bool operator !=(DomainKeyWrapper<T> x, DomainKeyWrapper<T> y)
        {
            return !(x == y);
        }




        public static bool operator ==(object x, DomainKeyWrapper<T> y)
        {
            if (x is DomainKeyWrapper<T> domainKey)
                return domainKey == x;
            else
                return false;
        }

        public static bool operator ==(DomainKeyWrapper<T> x, object y)
        {
            return y == x;
        }

        public static bool operator !=(object x, DomainKeyWrapper<T> y)
        {
            return !(x == y);
        }

        public static bool operator !=(DomainKeyWrapper<T> x, object y)
        {
            return !(x == y);
        }

        public static bool operator ==(T x, DomainKeyWrapper<T> y)
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

        public static bool operator ==(DomainKeyWrapper<T> x, T y)
        {
            return y == x;
        }

        public static bool operator !=(T x, DomainKeyWrapper<T> y)
        {
            return !(x == y);
        }

        public static bool operator !=(DomainKeyWrapper<T> x, T y)
        {
            return !(x == y);
        }
    }

    public class DomainKey : DomainValue
    {
        protected override bool GetIsShallow()
        {
            return true;
        }
        
        //private const int InitialHash = 23;
        //private const int HashMultiplier = 31;

        //public override bool Equals(object obj)
        //{
        //    if (obj == null)
        //        return false;

        //    if (this.GetType() == obj.GetType())
        //    {
        //        foreach (var prop in this.GetType().GetPropertiesEx())
        //        {
        //            if (!prop.CanGet || !PropertyEquals(prop.Get(this),prop.Get(obj)))
        //                return false;
        //        }
        //    }
        //    else
        //    {
        //        var leftProps = this.GetType().GetPropertiesEx().ToList();
        //        var rightProps = obj.GetType().GetPropertiesEx().ToList();

        //        if (leftProps.Count != rightProps.Count)
        //            return false;

        //        foreach (var prop in leftProps.Zip(leftProps, (x, y) => new { Left = x, Right = y }))
        //        {
        //            if (prop.Left.PropertyInfo.Name != prop.Right.PropertyInfo.Name
        //                || (!prop.Left.CanGet || !prop.Right.CanGet)
        //                || !PropertyEquals(prop.Left.Get(this), prop.Right.Get(obj)))
        //                return false;
        //        }
        //    }
        //    return true;
        //}

        //private bool PropertyEquals(object left, object right)
        //{
        //    if (left == null && right == null)
        //        return true;
        //    else if (left == null || right == null)
        //        return false;
        //    else
        //        return left.Equals(right);
        //}

        //public override int GetHashCode()
        //{
        //    unchecked
        //    {
        //        var properties = this.GetType().GetPropertiesEx().ToList();

        //        if (properties.Count == 0)
        //            return 0;

        //        // It's possible for two objects to return the same hash code based on 
        //        // identically valued properties, even if they're of two different types, 
        //        // so we include the object's type in the hash calculation

        //        //Removed
        //        //int hashCode = this.GetType().GetHashCode();

        //        int hashCode = InitialHash;

        //        foreach (var prop in properties.Where(x => x.CanGet && x.PropertyInfo.GetIndexParameters().Length == 0))
        //        {
        //            object value = prop.Get(this);
        //            if (value != null)
        //                hashCode = (hashCode * HashMultiplier) ^ value.GetHashCode();
        //        }

        //        return hashCode;
        //    }
        //}
  

        //public static bool operator ==(DomainKey x, DomainKey y)
        //{
        //    if (x is null && y is null)
        //        return true;

        //    if (x is null || y is null)
        //        return false;

        //    return x.Equals(y);
        //}

        //public static bool operator ==(object x, DomainKey y)
        //{
        //    if (x is DomainKey domainKey)
        //        return domainKey == x;
        //    else
        //        return false;
        //}

        //public static bool operator ==(DomainKey x, object y)
        //{
        //    if (y is DomainKey domainKey)
        //        return domainKey == x;
        //    else
        //        return false;
        //}

        //public static bool operator !=(DomainKey x, DomainKey y)
        //{
        //    return !(x == y);
        //}

        //public static bool operator !=(object x, DomainKey y)
        //{
        //    return !(x == y);
        //}

        //public static bool operator !=(DomainKey x, object y)
        //{
        //    return !(x == y);
        //}
    }
}
