using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamix.Reflection;

namespace DomainObjects.Core
{
    public interface IChangeTracker
    {
        void ResetChanges();
        void AcceptChanges();
        bool GetIsChanged();
        IReadOnlyDictionary<string, object> GetChanges();
        void MarkChanged();
        void MarkUnchanged();
        void BeginTracking();
        void StopTracking();
    }
    public class ChangeTracker : IChangeTracker
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        readonly object trackable;
        Type trackableType;
        bool suppressPropertyChanged;
        bool enabled;
        bool isChanged;
        bool? markedChanged;
        Dictionary<string, object> originalValues;
        Dictionary<string, object> changes = new Dictionary<string, object>();

        public ChangeTracker(object trackable)
        {
            this.trackable = trackable;
            trackableType = trackable.GetType();
            originalValues = GetCurrentValues();
        }

        private Dictionary<string, object> GetCurrentValues()
        {
            return trackableType
                .GetPropertiesEx(BindingFlagsEx.All)
                .Where(x => x.CanGet && x.CanSet)
                .ToDictionary(x => x.Name, x => x.Get(trackable));
        }

        private void SetOriginalValues()
        {
            foreach (var prop in trackableType
                    .GetPropertiesEx(BindingFlagsEx.All)
                    .Where(x => x.CanGet && x.CanSet))
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

        public IReadOnlyDictionary<string, object> GetChanges()
        {
            return new Dictionary<string, object>(changes);
        }

        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (suppressPropertyChanged || !enabled)
                return;

            if (!object.Equals(after, originalValues[propertyName]))
            {
                isChanged = true;
                changes[propertyName] = after;
            }
            else
            {
                isChanged = false;
                changes = new Dictionary<string, object>();
                foreach (var prop in trackableType.GetPropertiesExDic(BindingFlagsEx.All))
                {
                    var v = prop.Value.Get(trackable);
                    if (!object.Equals(v, originalValues[prop.Key]))
                    {
                        isChanged = true;
                        changes[prop.Key] = v;
                    }
                }
            }
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void BeginTracking()
        {
            enabled = true;
        }

        public void StopTracking()
        {
            enabled = false;
        }

        public void MarkChanged()
        {
            markedChanged = true;
        }

        public void MarkUnchanged()
        {
            markedChanged = false;
        }
    }


    public class TrackableBase : IChangeTracker
    {
        ChangeTracker tracker;

        public TrackableBase()
        {
            tracker = new ChangeTracker(this);
        }

        public void ResetChanges() => tracker.ResetChanges();
        public void AcceptChanges() => tracker.AcceptChanges();
        public bool GetIsChanged() => tracker.GetIsChanged();
        public IReadOnlyDictionary<string, object> GetChanges() => tracker.GetChanges();

        protected virtual void OnPropertyChanged(string propertyName, object before, object after)
        {
            //TODO: use tracker events to call this in a before/after manner
        }

        public void BeginTracking() => tracker.BeginTracking();
        public void StopTracking() => tracker.StopTracking();

        public void MarkChanged() => tracker.MarkChanged();
        public void MarkUnchanged() => tracker.MarkUnchanged();
    }


    public class Trackable //: INotifyPropertyChanged
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        bool suppressPropertyChanged;
        Dictionary<string, object> originalValues;
        Dictionary<string, object> changes = new Dictionary<string, object>();
        public Trackable()
        {
            originalValues = GetCurrentValues();
        }

        private Dictionary<string, object> GetCurrentValues()
        {
            return this.GetType()
                .GetPropertiesEx(BindingFlagsEx.All)
                .Where(x => x.CanGet && x.CanSet)
                .ToDictionary(x => x.Name, x => x.Get(this));
        }

        private void SetOriginalValues()
        {
            foreach (var prop in this.GetType()
                    .GetPropertiesEx(BindingFlagsEx.All)
                    .Where(x => x.CanGet && x.CanSet))
                prop.Set(this, originalValues[prop.Name]);
        }

        public void ResetChanges()
        {
            suppressPropertyChanged = true;
            try
            {
                SetOriginalValues();
                changes = new Dictionary<string, object>();
                isChanged = false;
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
        }

        private bool isChanged;

        public bool GetIsChanged()
        {
            return isChanged;
        }

        protected virtual void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (suppressPropertyChanged)
                return;

            if (!object.Equals(after, originalValues[propertyName]))
            {
                isChanged = true;
                changes[propertyName] = after;
            }
            else
            {
                isChanged = false;
                changes = new Dictionary<string, object>();
                foreach (var prop in this.GetType().GetPropertiesExDic(BindingFlagsEx.All))
                {
                    var v = prop.Value.Get(this);
                    if (!object.Equals(v, originalValues[prop.Key]))
                    {
                        isChanged = true;
                        changes[prop.Key] = v;
                    }
                }
            }
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

