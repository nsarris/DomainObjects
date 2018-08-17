using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Core
{
    internal enum ListItemChangeEnum
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

        internal ListChangedEventArgs(T item, ListItemChangeEnum change)
        {
            var value = item == null ? Enumerable.Empty<T>() : new[] { item };

            if (change == ListItemChangeEnum.Added)
                this.AddedItems = value;
            else if (change == ListItemChangeEnum.Removed)
                this.RemovedItems = value;
        }
    }


    public class TrackableReadOnlyList<T> : IReadOnlyList<T>//, IChangeTracker
    {
        protected readonly List<T> internalList;
        
        public TrackableReadOnlyList()
        {
            internalList = new List<T>();
        }

        public TrackableReadOnlyList(IList<T> list)
        {
            internalList = list.ToList();
        }

        public TrackableReadOnlyList(IEnumerable<T> collection)
        {
            internalList = new List<T>(collection);
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

        public virtual bool GetIsChangedDeep()
        {
            return GetIsChangedInner();
        }

        protected bool GetIsChangedInner()
        {
            if (typeof(IChangeTracker).IsAssignableFrom(typeof(T)))
                foreach (var item in internalList)
                    if ((item as IChangeTracker).GetIsChanged())
                        return true;

            return false;
        }

        public virtual void ResetChanges()
        {
            
        }

        public virtual void AcceptChanges()
        {
            
        }
    }



    public class TrackableList<T> : TrackableReadOnlyList<T>, IList<T> //, IChangeTracker
    {
        private readonly List<T> addedItems = new List<T>();
        private readonly List<T> removedItems = new List<T>();
        
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
        
        private void OnListChangedInternal(T item, ListItemChangeEnum change)
        {
            if (change == ListItemChangeEnum.Added)// && !addedItems.Contains(item))
                addedItems.Add(item);
            else if (change == ListItemChangeEnum.Removed)// && !removedItems.Contains(item))
                removedItems.Add(item);

            ListChanged?.Invoke(this, new ListChangedEventArgs<T>(item, change));
        }

        private void OnListChangedInternal(
            IEnumerable<T> added,
            IEnumerable<T> removed,
            bool cleared = false)
        {

            if (added != null)
                addedItems.AddRange(added);

            if (removed != null)
                removedItems.AddRange(removed);

            ListChanged?.Invoke(this, new ListChangedEventArgs<T>(added, removed, cleared));
        }

        protected virtual void OnListChanged(ListChangedEventArgs<T> eventArgs)
        {
            ListChanged?.Invoke(this, eventArgs);
        }

        public void Insert(int index, T item)
        {
            internalList.Insert(index, item);

            OnListChangedInternal(item, ListItemChangeEnum.Added);
        }

        public void RemoveAt(int index)
        {
            var item = internalList[index];
            internalList.RemoveAt(index);

            OnListChangedInternal(item, ListItemChangeEnum.Removed);
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
            OnListChangedInternal(item, ListItemChangeEnum.Added);
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
                OnListChangedInternal(item, ListItemChangeEnum.Removed);
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
            return addedItems.Any() || removedItems.Any();
        }

        public override void ResetChanges()
        {
            addedItems.ForEach(x => internalList.Remove(x));
            addedItems.ForEach(x => internalList.Add(x));
        }

        public override void AcceptChanges()
        {
            addedItems.Clear();
            removedItems.Clear();
        }

        public override bool GetIsChangedDeep()
        {
            return GetIsChanged() || base.GetIsChangedDeep();
        }
    }
}


