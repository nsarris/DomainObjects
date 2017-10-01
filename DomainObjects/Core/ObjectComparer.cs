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
        List<object> objectsChecked = new List<object>();

        public bool DeepEquals(object left, object right)
        {
            objectsChecked.Clear();
            var r = DeepEqualsInternal(left, right);
            objectsChecked.Clear();
            return r;
        }

        private bool DeepEqualsInternal(object left, object right)
        {
            //Circular dependency guard
            if (objectsChecked.Contains(left))
                return true;
            
            objectsChecked.Add(left);

            if (right == null && left == null)
                return true;
            
            if (right == null || left == null)
                return false;
            
            //Check type
            var leftType = right.GetType();
            var rightType = left.GetType();
            if (leftType != rightType)
                return false;
            
            //Primitives or values types
            if (leftType.IsPrimitive || leftType.IsValueType)
                return right.Equals(left);

            //Use IEquitable
            if (IsIEquatable(leftType))
            {
                var m = leftType.GetInterfaceMap(typeof(IEquatable<>).MakeGenericType(leftType)).TargetMethods.Where(x => x.Name == "Equals").FirstOrDefault();
                return ((bool)m.Invoke(left, new object[] { right }));
            }

            //If enumerable compare contents (assumes same positions)
            var leftEnumerable = right as IEnumerable<Object>;
            var rightEnumerable = left as IEnumerable<Object>;
            if ((leftEnumerable != null) && (rightEnumerable != null))
            {
                // Check size first
                if (leftEnumerable.Count() != rightEnumerable.Count())
                    return false;
                
                // Check items in order, assuming order is the same
                foreach(var items in leftEnumerable.Zip(rightEnumerable,(Left,Right) => new { Left, Right }))
                    if (!DeepEqualsInternal(items.Left, items.Right))
                        return false;
                
                return true;
            }

            // Compare object properties
            foreach (var prop in leftType.GetPropertiesEx())
            {
                // Skip properties that can't be read or require parameters
                if ((!prop.CanGet) || (prop.PropertyInfo.GetIndexParameters().Length != 0))
                    continue;

                if (!DeepEqualsInternal(prop.Get(left), prop.Get(right)))
                    return false;
            }

            return true;
        }

        private static Boolean IsIEquatable(Type type)
        {
            return type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEquatable<>) && x.GenericTypeArguments[0] == type);
        }
    }
}
