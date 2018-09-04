using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamix;
using Dynamix.Reflection;

namespace DomainObjects.Internal
{
    internal class ObjectComparer
    {
        public static ObjectComparer Default { get; } = new ObjectComparer();

        public bool DeepEquals(object left, object right)
        {
            var r = DeepEqualsInternal(left, right, new List<object>());
            return r;
        }

        public bool ShallowEquals(object left, object right)
        {
            if (left is null && right is null)
                return true;

            if (right == null || left == null)
                return false;

            var leftType = right.GetType();
            var rightType = left.GetType();

            if (leftType != rightType)
                return false;

            if (leftType.IsValueType)
                return Equals(left, right);

            if (ReferenceEquals(left, right))
                return true;

            foreach (var prop in leftType.GetPropertiesEx().Where(x => x.CanGet && x.PropertyInfo.GetIndexParameters().Length == 0))
                if (!PropertyEquals(prop.Get(left), prop.Get(right)))
                    return false;
            
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

        private bool DeepEqualsInternal(object left, object right, List<object> objectsChecked)
        {
            //Circular dependency guard
            if (objectsChecked.Contains(left))
                return true;

            objectsChecked.Add(left);

            if (ReferenceEquals(left, right))
                return true;

            if (right == null || left == null)
                return false;

            //Check type
            var leftType = right.GetType();
            var rightType = left.GetType();
            if (leftType != rightType)
                return false;

            //values types
            if (leftType.IsValueType)
                return Equals(left, right);

            //If enumerable compare contents (assumes same positions)
            if ((left is IEnumerable<Object> leftEnumerable)
                && (right is IEnumerable<Object> rightEnumerable))
            {
                // Check size first
                if (leftEnumerable.Count() != rightEnumerable.Count())
                    return false;

                // Check items in order, assuming order is the same
                foreach (var items in leftEnumerable.Zip(rightEnumerable, (Left, Right) => new { Left, Right }))
                    if (!DeepEqualsInternal(items.Left, items.Right, objectsChecked))
                        return false;

                return true;
            }

            // Compare object properties
            foreach (var prop in leftType.GetPropertiesEx().Where(x => x.CanGet && x.PropertyInfo.GetIndexParameters().Length == 0))
            {
                if (prop.Type.IsClass || prop.Type.IsInterface)
                {
                    if (!DeepEqualsInternal(prop.Get(left), prop.Get(right), objectsChecked))
                        return false;
                }
                else
                {
                    if (!Equals(prop.Get(right), prop.Get(left)))
                        return false;
                }
            }

            return true;
        }
    }
}
