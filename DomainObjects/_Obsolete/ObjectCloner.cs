using Dynamix;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dynamix.Reflection;

namespace DomainObjects.Core
{
    public static class ObjectCloner
    {
        public static T Clone<T>(T source)
        {
            return (T)new ObjectClonerVisitor().GetClone(source);
        }
    }

    public class ObjectClonerVisitor
    {
        private object CloneObject(object sourceObject)
        {
            var cloneObject = Activator.CreateInstance(sourceObject.GetType());
            foreach (var prop in sourceObject.GetType().GetPropertiesEx())
                VisitProperty(sourceObject, cloneObject, prop);
            return cloneObject;
        }

        public virtual object GetClone(object source)
        {
            return CloneObject(source);
        }

        public virtual void VisitValueProperty(object currentSource, object currentClone, PropertyInfoEx prop)
        {
            object value = prop.Get(currentSource, true);
            prop.Set(currentClone, value, true);
        }

        public virtual void VisitReferenceProperty(object currentSource, object currentClone, PropertyInfoEx prop)
        {
            object value = prop.Get(currentSource, true);
            prop.Set(currentClone, CloneObject(value), true);
        }

        public virtual void VisitCollectionProperty(object currentSource, object currentClone, PropertyInfoEx prop)
        {
            var value = prop.Get(currentSource);
            prop.Set(currentClone, ((IEnumerable)value).Clone(), true);
        }

        public virtual void VisitProperty(object currentSource,object currentClone, PropertyInfoEx prop)
        {
            var propType = prop.PropertyInfo.PropertyType;

            if (!prop.IsEnumerable)
            {
                if (!propType.IsValueType && !propType.IsEnum
                    && propType != typeof(string))
                    VisitReferenceProperty(currentSource, currentClone, prop);
                else
                    VisitValueProperty(currentSource, currentClone, prop);
            }
            else
                VisitCollectionProperty(currentSource, currentClone, prop);
        }
    }
}
