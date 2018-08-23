using System.Collections.Generic;

namespace DomainObjects.ChangeTracking
{
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

