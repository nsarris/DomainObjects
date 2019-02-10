using Dynamix.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.ChangeTracking
{
    internal class TrackableVisitor
    {
        public readonly static TrackableVisitor Instance = new TrackableVisitor();

        public bool GetIsChangedDeep(ITrackable trackable)
        {
            return GetIsChangedDeep(trackable, new HashSet<object>());
        }

        private bool GetIsChangedDeep(ITrackable trackable, HashSet<object> visited)
        {
            if (trackable == null)
                return false;

            var trackedObject = trackable is ChangeTracker changeTracker
                ? changeTracker.GetTrackedObject()
                : trackable;

            if (trackedObject == null || visited.Contains(trackedObject))
                return false;

            visited.Add(trackedObject);

            if (trackedObject != trackable && trackedObject is ITrackable innerTrackable)
            {
                if (innerTrackable.GetIsChanged())
                    return true;
            }
            else if (trackable.GetIsChanged())
                return true;

            foreach (var prop in trackedObject.GetType().GetPropertiesEx().Where(x => x.CanGet && x.PropertyInfo.GetIndexParameters().Length == 0))
            {
                if (prop.Type.IsAssignableTo(typeof(ITrackableObject))
                    && GetIsChangedDeep(prop.Get(trackedObject) as ITrackableObject, visited))
                {
                    return true;
                }
                else if (prop.Type.IsAssignableTo(typeof(ITrackableCollection)))
                {
                    if (!(prop.Get(trackedObject) is ITrackableCollection trackableList))
                        return false;

                    if (trackableList.GetIsChanged())
                        return true;

                    if (trackableList.ElementType.IsAssignableTo(typeof(ITrackableObject)))
                        foreach (ITrackableObject item in trackableList)
                            if (GetIsChangedDeep(item, visited))
                                return true;
                }
                else if (prop.IsEnumerable)
                {
                    foreach (var item in (IEnumerable)prop.Get(trackedObject))
                    {
                        if (item != null && item is ITrackableObject enumeratedTrackable
                            && GetIsChangedDeep(enumeratedTrackable, visited))
                            return true;
                    }
                }
            }

            return false;
        }

        public void Visit(ITrackable trackable, Action<ITrackable> action)
        {
            Visit(trackable, action, new HashSet<object>());
        }

        private void Visit(ITrackable trackable, Action<ITrackable> action, HashSet<object> visited)
        {
            if (trackable == null)
                return;

            var trackedObject = trackable is ChangeTracker changeTracker
                ? changeTracker.GetTrackedObject()
                : trackable;

            if (trackedObject == null || visited.Contains(trackedObject))
                return;

            visited.Add(trackedObject);

            if (trackedObject != trackable && trackedObject is ITrackable innerTrackable)
                action(innerTrackable);
            else
                action(trackable);

            foreach (var prop in trackedObject.GetType().GetPropertiesEx().Where(x => x.CanGet && x.PropertyInfo.GetIndexParameters().Length == 0))
            {
                if (prop.Type.IsAssignableTo(typeof(ITrackableObject)))
                {
                    Visit(prop.Get(trackedObject) as ITrackableObject, action, visited);
                }
                else if (prop.Type.IsAssignableTo(typeof(ITrackableCollection))
                    && prop.Get(trackedObject) is ITrackableCollection trackableList)
                {
                    action(trackableList);

                    if (trackableList.ElementType.IsAssignableTo(typeof(ITrackableObject)))
                        foreach (ITrackableObject item in trackableList)
                            Visit(item, action, visited);
                }
                else if (prop.IsEnumerable)
                {
                    foreach (var item in (IEnumerable)prop.Get(trackedObject))
                    {
                        if (item != null && item is ITrackableObject enumeratedTrackable)
                            Visit(enumeratedTrackable, action, visited);
                    }
                }
            }
        }
    }
}

