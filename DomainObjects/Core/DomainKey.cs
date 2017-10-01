using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamix;
using Dynamix.Reflection;

namespace DomainObjects.Core
{
    public class DomainKey : DynamicType
    {
        private const int HashMultiplier = 31;

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (this.GetType() == obj.GetType())
            {
                foreach (var prop in this.GetType().GetPropertiesEx())
                {
                    if (!prop.CanGet || !PropertyEquals(prop.Get(this),prop.Get(obj)))
                        return false;
                }
            }
            else
            {
                var leftProps = this.GetType().GetPropertiesEx().ToList();
                var rightProps = obj.GetType().GetPropertiesEx().ToList();

                if (leftProps.Count != rightProps.Count)
                    return false;

                foreach (var prop in leftProps.Zip(leftProps, (x, y) => new { Left = x, Right = y }))
                {
                    if (prop.Left.PropertyInfo.Name != prop.Right.PropertyInfo.Name
                        || (!prop.Left.CanGet || !prop.Right.CanGet)
                        || !PropertyEquals(prop.Left.Get(this), prop.Right.Get(obj)))
                        return false;
                }
            }
            return true;
        }

        private bool PropertyEquals(object left, object right)
        {
            if (left == null && right == null)
                return true;
            else if (left == null || right == null)
                return false;
            else
                return left.Equals(right);
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

        public static bool operator ==(DomainKey x, DomainKey y)
        {
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
                return false;

            return x.Equals(y);
        }

        public static bool operator ==(object x, DomainKey y)
        {
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
                return false;

            return y.Equals(x);
        }

        public static bool operator ==(DomainKey x,object  y)
        {
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
                return false;

            return x.Equals(y);
        }

        public static bool operator !=(DomainKey x, DomainKey y)
        {
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
                return true;

            return !x.Equals(y);
        }

        public static bool operator !=(object x, DomainKey y)
        {
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
                return true;

            return !x.Equals(y);
        }

        public static bool operator !=(DomainKey x,object  y)
        {
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
                return true;

            return !x.Equals(y);
        }
    }
}
