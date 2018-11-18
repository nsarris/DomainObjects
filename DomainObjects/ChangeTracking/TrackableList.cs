using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.ChangeTracking
{
    internal enum ListItemChange
    {
        Added,
        Removed
    }

    public class ListChangedEventArgs<T> : EventArgs
    {
        public IEnumerable<T> AddedItems { get; }
        public IEnumerable<T> RemovedItems { get; }

        internal ListChangedEventArgs(IEnumerable<T> addedItems, IEnumerable<T> removedItems, bool listCleared = false)
        {
            this.AddedItems = addedItems ?? Enumerable.Empty<T>();
            this.RemovedItems = removedItems ?? Enumerable.Empty<T>();
        }

        internal ListChangedEventArgs(T item, ListItemChange change)
        {
            var value = item == null ? Enumerable.Empty<T>() : new[] { item };

            if (change == ListItemChange.Added)
                this.AddedItems = value;
            else if (change == ListItemChange.Removed)
                this.RemovedItems = value;
        }
    }


    public class TrackableReadOnlyList<T> : IReadOnlyList<T>, ITrackableCollection<T>
    {
        protected readonly List<T> internalList;
        
        public TrackableReadOnlyList()
        {
            internalList = new List<T>();
        }

        public TrackableReadOnlyList(IList<T> list)
        {
            internalList = list?.ToList() ?? new List<T>();
        }

        public TrackableReadOnlyList(IEnumerable<T> collection)
        {
            internalList = new List<T>(collection ?? Enumerable.Empty<T>());
        }

        public int IndexOf(T item)
        {
            return internalList.IndexOf(item);
        }
        
        public T this[int index]
        {
            get { return internalList[index]; }
        }

        public bool Contains(T item)
        {
            return internalList.Contains(item);
        }

        public int Count
        {
            get { return internalList.Count; }
        }

        public Type ElementType => typeof(T);

        public IEnumerator<T> GetEnumerator()
        {
            return internalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)internalList).GetEnumerator();
        }

        public static implicit operator List<T>(TrackableReadOnlyList<T> t)
        {
            return new List<T>(t);
        }

        public static implicit operator TrackableReadOnlyList<T>(List<T> list)
        {
            return new TrackableReadOnlyList<T>(list);
        }

        public virtual bool GetIsChanged()
        {
            return false;
        }

        public bool GetIsChangedDeep()
        {
            if (GetIsChanged())
                return true;

            if (ElementTypeIsTrackable)
                foreach (var item in internalList)
                    if ((item as ITrackable).GetIsChangedDeep())
                        return true;

            return false;
        }

        public bool ElementTypeIsTrackable { get; } = typeof(ITrackable).IsAssignableFrom(typeof(T));
        

        public virtual void ResetChanges()
        {
            
        }

        public virtual void AcceptChanges()
        {
            
        }

        public virtual IEnumerable<T> GetAdded()
        {
            return Enumerable.Empty<T>();
        }

        public virtual IEnumerable<T> GetRemoved()
        {
            return Enumerable.Empty<T>();
        }

        public virtual void MarkChanged()
        {
            
        }

        public virtual void MarkUnchanged()
        {
            
        }

        public virtual void BeginTracking()
        {
            
        }

        public virtual void StopTracking()
        {
            
        }

        private void ForEachTrackable(Action<ITrackable> action)
        {
            foreach (var item in this)
                if (item is ITrackableObject trackable)
                    action(trackable);
        }

        public void ResetChangesDeep()
        {
            ResetChanges();
            ForEachTrackable(x => x.ResetChangesDeep());
        }

        public void AcceptChangesDeep()
        {
            AcceptChanges();
            ForEachTrackable(x => x.AcceptChangesDeep());
        }

        public void BeginTrackingDeep()
        {
            BeginTracking();
            ForEachTrackable(x => x.BeginTrackingDeep());
        }

        public void StopTrackingDeep()
        {
            StopTracking();
            ForEachTrackable(x => x.StopTrackingDeep());
        }

        public void MarkChangedDeep()
        {
            MarkChanged();
            ForEachTrackable(x => x.MarkChangedDeep());
        }

        public void MarkUnchangedDeep()
        {
            MarkUnchanged();
            ForEachTrackable(x => x.MarkUnchangedDeep());
        }

        IEnumerable<object> ITrackableCollection.GetAdded()
        {
            return GetAdded().Cast<object>();
        }

        IEnumerable<object> ITrackableCollection.GetRemoved()
        {
            return GetRemoved().Cast<object>();
        }
    }



    public class TrackableList<T> : TrackableReadOnlyList<T>, IList<T> //, IChangeTracker
    {
        private readonly List<T> addedItems = new List<T>();
        private readonly List<T> removedItems = new List<T>();
        private bool enabled;
        private bool? markedChanged;

        public delegate void ListChangedEventHandler(TrackableList<T> source, ListChangedEventArgs<T> e);

        public event ListChangedEventHandler ListChanged;

        public TrackableList()
            :base()
        {
            
        }

        public TrackableList(IList<T> list)
            :base(list)
        {
            
        }

        public TrackableList(IEnumerable<T> collection)
            :base(collection)
        {
            
        }
        
        private void OnListChangedInternal(T item, ListItemChange change)
        {
            if (!enabled)
                return; 

            if (change == ListItemChange.Added)// && !addedItems.Contains(item))
                addedItems.Add(item);
            else if (change == ListItemChange.Removed)// && !removedItems.Contains(item))
                removedItems.Add(item);

            ListChanged?.Invoke(this, new ListChangedEventArgs<T>(item, change));
        }

        private void OnListChangedInternal(
            IEnumerable<T> added,
            IEnumerable<T> removed,
            bool cleared = false)
        {
            if (!enabled)
                return;

            if (added != null)
                addedItems.AddRange(added);

            if (removed != null)
                removedItems.AddRange(removed);

            ListChanged?.Invoke(this, new ListChangedEventArgs<T>(added, removed, cleared));
        }
        
        public void Insert(int index, T item)
        {
            internalList.Insert(index, item);

            OnListChangedInternal(item, ListItemChange.Added);
        }

        public void RemoveAt(int index)
        {
            var item = internalList[index];
            internalList.RemoveAt(index);

            OnListChangedInternal(item, ListItemChange.Removed);
        }

        public void Remove(Func<T, bool> predicate)
        {
            var items = internalList.Where(predicate).ToList();
            var removed = new List<T>();
            foreach (var item in items)
                if (internalList.Remove(item))
                    removed.Add(item);

            OnListChangedInternal(
                added: null,
                removed: items);
        }

        public new T this[int index]
        {
            get { return base[index]; }
            set
            {
                var previous = internalList[index];
                if (!Equals(previous, value))
                {
                    internalList[index] = value;
                    OnListChangedInternal(
                        added: new T[] { value },
                        removed: new T[] { previous });
                }
            }
        }

        public void Add(T item)
        {
            internalList.Add(item);
            OnListChangedInternal(item, ListItemChange.Added);
        }

        public void AddRange(IEnumerable<T> items)
        {
            internalList.AddRange(items);
            OnListChangedInternal(
                    added: items,
                    removed: null);
        }

        public void Clear()
        {
            var items = internalList.ToList();
            internalList.Clear();

            OnListChangedInternal(
                added: null,
                removed: items,
                cleared: true);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            internalList.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            if (internalList.Remove(item))
            {
                OnListChangedInternal(item, ListItemChange.Removed);
                return true;
            }
            else
                return false;
        }

        public static implicit operator List<T>(TrackableList<T> t)
        {
            return new List<T>(t);
        }

        public static implicit operator TrackableList<T>(List<T> list)
        {
            return new TrackableList<T>(list);
        }

        public override bool GetIsChanged()
        {
            if (markedChanged.HasValue)
                return markedChanged == true;
            else
                return addedItems.Any() || removedItems.Any();
        }

        public override void ResetChanges()
        {
            addedItems.ForEach(x => internalList.Remove(x));
            removedItems.ForEach(x => internalList.Add(x));
        }

        public override void AcceptChanges()
        {
            addedItems.Clear();
            removedItems.Clear();
        }

        public override IEnumerable<T> GetAdded()
        {
            return addedItems;
        }

        public override IEnumerable<T> GetRemoved()
        {
            return removedItems;
        }

        public override void BeginTracking()
        {
            enabled = true;
        }

        public override void StopTracking()
        {
            enabled = false;
        }

        public override void MarkChanged()
        {
            markedChanged = true;
        }

        public override void MarkUnchanged()
        {
            markedChanged = false;
        }
    }
}


