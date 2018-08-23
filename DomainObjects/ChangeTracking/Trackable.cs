using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.ChangeTracking
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
}

