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
            //object enumerable = prop.Get(currentSource, true);

            ////Clone List -> depends on List/Collection/Dictionary/Array or readonly etc
            ////Need ListOf (generic like, untyped)
            //if (list != null)
            //{
            //    value = Activator.CreateInstance(list.GetType());
            //    var l = new List<object>();
            //    foreach (var item in prop.EnumerableDescriptor.AsEnumerable(list))
            //    {
            //        object cloneItem = null;

            //        if (!propType.IsValueType && !propType.IsEnum
            //        && propType != typeof(string))
            //            cloneItem = CloneObject(item);
            //        else
            //            cloneItem = item;

            //        l.Add(cloneItem);
            //    }
            //}

            var value = prop.Get(currentSource);

            if (value != null)
            {
                var clonerEnumerable = prop.EnumerableDescriptor.IsGeneric ?
                    CollectionClonerEnumerable.CreateGeneric(prop.EnumerableDescriptor.ElementType, currentSource, prop, CloneCollectionItem) :
                    new CollectionClonerEnumerable(currentSource, prop, CloneCollectionItem);

                value = prop.EnumerableDescriptor.CreateFromEnumerable(clonerEnumerable);
            }
                
            prop.Set(currentClone, value, true);
        }

        private class CollectionClonerEnumerable : IEnumerable
        {
            protected object currentSource;
            protected PropertyInfoEx prop;
            protected Func<object,PropertyInfoEx, object> cloneMethod;
            public CollectionClonerEnumerable(object currentSource, PropertyInfoEx prop, Func<object, PropertyInfoEx, object> cloneMethod)
            {
                this.currentSource = currentSource;
                this.prop = prop;
            
                this.cloneMethod = cloneMethod;
            }

            public IEnumerator GetEnumerator()
            {
                object enumerable = prop.Get(currentSource, true);

                foreach (var item in prop.EnumerableDescriptor.AsEnumerableOfElementType(enumerable))
                    yield return cloneMethod(item, prop);
            }

            public static CollectionClonerEnumerable CreateGeneric(Type TypeArgument, object currentSource, PropertyInfoEx prop, Func<object, PropertyInfoEx, object> cloneMethod)
            {
                return (CollectionClonerEnumerable)Activator.CreateInstance(typeof(CollectionClonerEnumerable<>).MakeGenericType(
                    new Type[] { TypeArgument }),new object[] { currentSource, prop, cloneMethod });
            }
        }

        private class CollectionClonerEnumerable<T> : CollectionClonerEnumerable, IEnumerable<T>
        {
            public CollectionClonerEnumerable(object currentSource, PropertyInfoEx prop, Func<object, PropertyInfoEx, object> cloneMethod)
                :base(currentSource, prop,cloneMethod)
            {

            }
            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                object enumerable = prop.Get(currentSource, true);

                foreach (var item in prop.EnumerableDescriptor.AsEnumerableOfElementType(enumerable))
                    yield return (T)cloneMethod(item, prop);
            }
        }

        private bool IsPrimitiveLike(Type t)
        {
            //Cache Types

            if (t.IsPrimitive || t.IsEnum || t == typeof(string) 
                || t == typeof(DateTime) || t == typeof(DateTime?)
                || t == typeof(TimeSpan) || t == typeof(TimeSpan?)
                || t == typeof(decimal) || t == typeof(decimal?)
                )
                return true;
            
            foreach(var prop in t.GetPropertiesEx())
            {
                if (!IsPrimitiveLike(prop.PropertyInfo.PropertyType))
                    return false;
            }

            return true;
        }

        

        private object CloneCollectionItem(object sourceItem, PropertyInfoEx enumerableProperty)
        {
            if (sourceItem == null)
                return null;

            if (!IsPrimitiveLike(sourceItem.GetType()))
                return CloneObject(sourceItem);
            else
                return sourceItem;

        }

        //private IEnumerable GetCollectionCloneEnumerable(object currentSource, PropertyInfoEx prop)
        //{
        //    object enumerable = prop.Get(currentSource, true);
        //    var propType = prop.PropertyInfo.PropertyType;

        //    foreach (var item in prop.EnumerableDescriptor.AsEnumerableOfElementType(enumerable))
        //        if (!propType.IsValueType && !propType.IsEnum
        //        && propType != typeof(string))
        //            yield return CloneObject(item);
        //        else
        //            yield return item;
        //}

        public virtual void VisitProperty(object currentSource,object currentClone, PropertyInfoEx prop)
        {
            object v = prop.Get(currentSource, true);
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
