﻿using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Core
{
    internal class TrackableVisitor 
    {
        public static TrackableVisitor Instance = new TrackableVisitor();

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
            else
                if (trackable.GetIsChanged())
                    return true;

            foreach(var prop in trackedObject.GetType().GetPropertiesEx().Where(x => x.CanGet && x.PropertyInfo.GetIndexParameters().Length == 0))
            {
                if (prop.Type.IsAssignableTo(typeof(ITrackableObject)))
                    if (GetIsChangedDeep(prop.Get(trackedObject) as ITrackableObject, visited))
                        return true;
                
                if (prop.Type.IsAssignableTo(typeof(ITrackableCollection)))
                {
                    var trackableList = prop.Get(trackedObject) as ITrackableCollection;

                    if (trackableList == null)
                        return false;

                    if ((prop.Get(trackedObject) as ITrackableCollection).GetIsChanged())
                        return true;

                    if (trackableList.ElementType.IsAssignableTo(typeof(ITrackableObject)))
                        foreach (ITrackableObject item in trackableList)
                            GetIsChangedDeep(item, visited);
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
                    Visit(prop.Get(trackedObject) as ITrackableObject, action, visited);
                        
                if (prop.Type.IsAssignableTo(typeof(ITrackableCollection)))
                {
                    var trackableList = prop.Get(trackedObject) as ITrackableCollection;

                    if (trackableList != null)
                    {
                        action(trackableList);

                        if (trackableList.ElementType.IsAssignableTo(typeof(ITrackableObject)))
                            foreach (ITrackableObject item in trackableList)
                                Visit(item, action, visited);
                    }
                }
            }
        }
    }
}
