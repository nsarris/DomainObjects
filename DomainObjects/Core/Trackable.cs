using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamix.Reflection;

namespace DomainObjects.Core
{
    public interface ITrackable
    {
        void ResetChanges();
        void AcceptChanges();
        bool GetIsChanged();
        void MarkChanged();
        void MarkUnchanged();
        void BeginTracking();
        void StopTracking();

        void ResetChangesDeep();
        void AcceptChangesDeep();
        bool GetIsChangedDeep();
        void MarkChangedDeep();
        void MarkUnchangedDeep();
        void BeginTrackingDeep();
        void StopTrackingDeep();
    }

    public interface ITrackableObject : ITrackable
    {
        IReadOnlyDictionary<string, object> GetChanges();
    }

    public interface ITrackableCollection : ITrackable, IEnumerable
    {
        Type ElementType { get; }
        IEnumerable<object> GetAdded();
        IEnumerable<object> GetRemoved();
    }

    public interface ITrackableCollection<out T> : ITrackableCollection
    {
        //GetChanges
        new IEnumerable<T> GetAdded();
        new IEnumerable<T> GetRemoved();
    }

    public class PropertyChangedExtendedEventArgs : EventArgs
    {
        public PropertyChangedExtendedEventArgs(string propertyName, object before, object after)
        {
            PropertyName = propertyName;
            Before = before;
            After = after;
        }
        public string PropertyName { get; }
        public object Before { get; }
        public object After { get; }
    }
    public delegate void PropertyChangedExtendedEventHandler(object sender, PropertyChangedExtendedEventArgs e);

    public class ChangeTracker : ITrackableObject, INotifyPropertyChanged
    {
        public event PropertyChangedExtendedEventHandler BeforePropertyChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedExtendedEventHandler AfterPropertyChanged;

        readonly object trackable;
        readonly Type trackableType;
        bool suppressPropertyChanged;
        bool enabled;
        bool isChanged;
        bool? markedChanged;
        Dictionary<string, object> originalValues;
        Dictionary<string, object> changes = new Dictionary<string, object>();

        public object GetTrackedObject() => trackable;

        public ChangeTracker(object trackable)
        {
            this.trackable = trackable;
            trackableType = trackable.GetType();
            originalValues = GetCurrentValues();
        }

        private IEnumerable<PropertyInfoEx> GetProperties()
        {
            return trackableType
                .GetPropertiesEx(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                .Where(x => x.CanGet && x.CanSet && x.PropertyInfo.GetIndexParameters().Length == 0);
        }

        private Dictionary<string, object> GetCurrentValues()
        {
            return GetProperties()
                .ToDictionary(x => x.Name, x => x.Get(trackable));
        }

        private void SetOriginalValues()
        {
            foreach (var prop in GetProperties())
                prop.Set(trackable, originalValues[prop.Name]);
        }

        public void ResetChanges()
        {
            suppressPropertyChanged = true;
            try
            {
                SetOriginalValues();
                changes = new Dictionary<string, object>();
                isChanged = false;
                markedChanged = null;
            }
            finally
            {
                suppressPropertyChanged = false;
            }
        }

        public void AcceptChanges()
        {
            originalValues = GetCurrentValues();
            changes = new Dictionary<string, object>();
            isChanged = false;
            markedChanged = null;
        }

        

        public bool GetIsChanged()
        {
            if (markedChanged.HasValue)
                return markedChanged == true;
            else
                return isChanged;
        }

        public bool GetIsChangedDeep()
        {
            return TrackableVisitor.Instance.GetIsChangedDeep(this);
        }

        public IReadOnlyDictionary<string, object> GetChanges()
        {
            return new Dictionary<string, object>(changes);
        }

        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (suppressPropertyChanged || !enabled)
                return;

            BeforePropertyChanged?.Invoke(this, new PropertyChangedExtendedEventArgs(propertyName, before, after));

            if (!object.Equals(after, originalValues[propertyName]))
            {
                isChanged = true;
                changes[propertyName] = after;
            }
            else
            {
                isChanged = false;
                changes = new Dictionary<string, object>();
                foreach (var prop in GetProperties())
                {
                    var v = prop.Get(trackable);
                    if (!object.Equals(v, originalValues[prop.Name]))
                    {
                        isChanged = true;
                        changes[prop.Name] = v;
                    }
                }
            }

            AfterPropertyChanged?.Invoke(this, new PropertyChangedExtendedEventArgs(propertyName, before, after));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void BeginTracking()
        {
            enabled = true;
        }

        public void StopTracking()
        {
            enabled = false;
        }

        public void BeginTrackingDeep()
        {
            TrackableVisitor.Instance.Visit(this, x => x.BeginTracking());
        }

        public void StopTrackingDeep()
        {
            TrackableVisitor.Instance.Visit(this, x => x.StopTracking());
        }

        public void MarkChanged()
        {
            markedChanged = true;
        }

        public void MarkUnchanged()
        {
            markedChanged = false;
        }

        public void ResetChangesDeep()
        {
            TrackableVisitor.Instance.Visit(this, x => x.ResetChanges());
        }

        public void AcceptChangesDeep()
        {
            TrackableVisitor.Instance.Visit(this, x => x.AcceptChanges());
        }

        public void MarkChangedDeep()
        {
            TrackableVisitor.Instance.Visit(this, x => x.MarkChanged());
        }

        public void MarkUnchangedDeep()
        {
            TrackableVisitor.Instance.Visit(this, x => x.MarkUnchanged());
        }
    }


    public abstract class TrackableBase : ITrackableObject
    {
        readonly ChangeTracker tracker;

        protected TrackableBase()
        {
            tracker = new ChangeTracker(this);
            tracker.BeforePropertyChanged += (object sender, PropertyChangedExtendedEventArgs e) => OnBeforePropertyChanged(e.PropertyName, e.Before, e.After);
            tracker.AfterPropertyChanged += (object sender, PropertyChangedExtendedEventArgs e) => OnAfterPropertyChanged(e.PropertyName, e.Before, e.After);
        }

        protected virtual void OnBeforePropertyChanged(string propertyName, object before, object after)
        {

        }

        protected virtual void OnAfterPropertyChanged(string propertyName, object before, object after)
        {

        }

        protected void OnPropertyChanged(string propertyName, object before, object after)
        {
            tracker.OnPropertyChanged(propertyName, before, after);
        }

        public IReadOnlyDictionary<string, object> GetChanges() => tracker.GetChanges();

        public void ResetChanges() => tracker.ResetChanges();
        public void AcceptChanges() => tracker.AcceptChanges();
        public bool GetIsChanged() => tracker.GetIsChanged();
        public void BeginTracking() => tracker.BeginTracking();
        public void StopTracking() => tracker.StopTracking();
        public void MarkChanged() => tracker.MarkChanged();
        public void MarkUnchanged() => tracker.MarkUnchanged();

        
        public void ResetChangesDeep() => tracker.ResetChangesDeep();
        public void AcceptChangesDeep() => tracker.AcceptChangesDeep();
        public bool GetIsChangedDeep() => tracker.GetIsChangedDeep();
        public void BeginTrackingDeep() => tracker.BeginTrackingDeep();
        public void StopTrackingDeep() => tracker.StopTrackingDeep();
        public void MarkChangedDeep() => tracker.MarkChangedDeep();
        public void MarkUnchangedDeep() => tracker.MarkUnchangedDeep();
    }
}

