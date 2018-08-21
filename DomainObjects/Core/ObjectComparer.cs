using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamix;
using Dynamix.Reflection;

namespace DomainObjects.Core
{
    public class ObjectComparer
    {
        readonly List<object> objectsChecked = new List<object>();

        public bool DeepEquals(object left, object right)
        {
            objectsChecked.Clear();
            var r = DeepEqualsInternal(left, right);
            objectsChecked.Clear();
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

            if (leftType.GetType().IsValueType)
                return Equals(left, right);

            if (ReferenceEquals(left, right))
                return true;

            foreach (var prop in leftType.GetPropertiesEx().Where(x => x.CanGet && x.PropertyInfo.GetIndexParameters().Length == 0))
                if (!PropertyEquals(right, left))
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

        private bool DeepEqualsInternal(object left, object right)
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

            //Use IEquitable
            //if (IsIEquatable(leftType))
            //{
            //    var m = leftType.GetInterfaceMap(typeof(IEquatable<>).MakeGenericType(leftType)).TargetMethods.FirstOrDefault(x => x.Name == "Equals");
            //    return ((bool)m.Invoke(left, new object[] { right }));
            //}

            //If enumerable compare contents (assumes same positions)
            if ((left is IEnumerable<Object> leftEnumerable)
                && (right is IEnumerable<Object> rightEnumerable))
            {
                // Check size first
                if (leftEnumerable.Count() != rightEnumerable.Count())
                    return false;

                // Check items in order, assuming order is the same
                foreach (var items in leftEnumerable.Zip(rightEnumerable, (Left, Right) => new { Left, Right }))
                    if (!DeepEqualsInternal(items.Left, items.Right))
                        return false;

                return true;
            }

            // Compare object properties
            foreach (var prop in leftType.GetPropertiesEx().Where(x => x.CanGet && x.PropertyInfo.GetIndexParameters().Length == 0))
            {
                if (prop.Type.IsClass)
                    if (!DeepEqualsInternal(prop.Get(left), prop.Get(right)))
                        return false;
                    else
                    if (!Equals(right, left))
                        return false;
            }

            return true;
        }

        //private static Boolean IsIEquatable(Type type)
        //{
        //    return type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEquatable<>) && x.GenericTypeArguments[0] == type);
        //}
    }
}
