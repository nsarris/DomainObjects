using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainObjects.Metadata;
using Dynamix;

namespace DomainObjects.Core
{
    public enum DomainObjectState
    {
        Uninitialized,
        New,
        Existing,
        Deleted
    }

    public abstract class DomainEntity : DomainObject, IKeyProvider, ITrackable
    {
        private DomainEntityMetadata entityMetadata;
        public DomainEntityMetadata GetEntityMetadata()
        {
            if (entityMetadata == null)
                entityMetadata = DomainModelMetadataRegistry.GetEntityDescriptor(this.GetType());

            return entityMetadata;
        }


        protected DomainEntity()
        {
            changeTracker = new ChangeTracker(this);
            changeTracker.BeforePropertyChanged += (object sender, PropertyChangedExtendedEventArgs e) => OnBeforePropertyChanged(e.PropertyName, e.Before, e.After);
            changeTracker.AfterPropertyChanged += (object sender, PropertyChangedExtendedEventArgs e) => OnAfterPropertyChanged(e.PropertyName, e.Before, e.After);
        }

        #region Key and Equality
        public object GetKey()
        {
            return GetEntityMetadata().GetKey(this);
            //return DomainEntityKeyProvider.GetKey(this);
        }

        public bool KeyEquals(DomainEntity other)
        {
            if (other == null || other.GetType() != this.GetType())
                return false;

            return GetKey().Equals(other.GetKey());
        }

        public void SetKey(params object[] values)
        {
            if (this.state != DomainObjectState.New)
                throw new InvalidOperationException("Entity keys can only be set after initialization on entities in New state");

            //If key property is DomainKey/DomainValue (only one allowed) 
            //    -> if one value of same type -> set
            //       else create new and try to set in sequence
            //if key properties are primitive properties -> try set each
        }


        //public override bool Equals(object obj)
        //{
        //    if (obj == null)
        //        return false;

        //    if (this.GetType() != obj.GetType())
        //        return false;

        //    return GetKey().Equals(((DomainEntity)obj).GetKey());
        //}

        //public override int GetHashCode()
        //{
        //    return GetKey().GetHashCode();
        //}

        //public static bool operator ==(DomainEntity x, DomainEntity y)
        //{
        //    if (x is null && y is null)
        //        return true;
        //    else if (x is null || y is null)
        //        return false;

        //    return x.Equals(y);
        //}

        //public static bool operator !=(DomainEntity x, DomainEntity y)
        //{
        //    if (x is null && y is null)
        //        return false;
        //    else if (x is null || y is null)
        //        return true;

        //    return !x.Equals(y);
        //}

        #endregion

        #region ObjectState

        private DomainObjectState state = DomainObjectState.Uninitialized;

        public DomainObjectState GetObjectState()
        {
            return state;
        }

        public void Init(bool isNew)
        {
            if (isNew)
                MarkNew();
            else
                MarkExisting();

            BeginTracking();
        }

        public void MarkNew()
        {
            state = DomainObjectState.New;
        }

        public void MarkExisting()
        {
            state = DomainObjectState.Existing;
        }

        public void MarkDeleted()
        {
            state = DomainObjectState.Deleted;
        }

        //protected void SetObjectState(DomainObjectState state)
        //{
        //    this.state = state;
        //}

        #endregion

        #region Change Tracking

        private readonly ChangeTracker changeTracker;

        public void MarkChanged()
        {
            changeTracker.MarkChanged();
        }

        public void MarkUnchanged()
        {
            changeTracker.MarkUnchanged();
        }

        public bool GetIsChanged()
        {
            return changeTracker.GetIsChanged();
        }

        public bool GetIsChangedDeep()
        {
            if (GetIsChanged())
                return true;

            foreach (var prop in entityMetadata.GetAggregateListProperties())
            {
                //enumerate list -> GetIsChangedDeep

            }

            foreach (var listProp in entityMetadata.GetValueListProperties())
            {
                var list = listProp.Property.Get(this) as ITrackableCollection;

                if (!listProp.IsImmutable && list.GetIsChanged())
                    return true;

                foreach (var item in list)
                {
                    if (item is ITrackable trackable && trackable.GetIsChanged())
                        return true;
                    else
                        break;
                }
            }

            return false;
            

            //Get collections ->
            //if collection of ITrackable -> check items internally
            //if also ITrackableCollection -> getdeleted (ignore added? throw incosistency error)
            //if collection if non ITrackable items 
            //if ITrackableCollection -> added / deleted 
            //else do nothing
        }



        public void ResetChanges()
        {
            changeTracker.ResetChanges();
        }

        public void AcceptChanges()
        {
            changeTracker.AcceptChanges();
        }

        public IReadOnlyDictionary<string, object> GetChanges()
        {
            return changeTracker.GetChanges();
        }

        public void BeginTracking()
        {
            changeTracker.BeginTracking();
        }

        public void StopTracking()
        {
            changeTracker.StopTracking();
        }

        protected void OnPropertyChanged(string propertyName, object before, object after)
        {
            changeTracker.OnPropertyChanged(propertyName, before, after);
        }

        protected virtual void OnBeforePropertyChanged(string propertyName, object before, object after)
        {

        }

        protected virtual void OnAfterPropertyChanged(string propertyName, object before, object after)
        {

        }


        #endregion

        #region Validation

        //public IEnumerable<ValidationResult> Validate()
        //{
        //    return Validate(null);
        //}

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    var result = new List<ValidationResult>();

        //    foreach (var prop in this.GetType().GetPropertiesEx())
        //    {
        //        var attrs = prop.PropertyInfo.GetCustomAttributes(typeof(ValidationAttribute), true).Cast<ValidationAttribute>();
        //        foreach (var a in attrs)
        //        {
        //            var v = prop.Get(this);

        //            if (!a.IsValid(v))
        //            {
        //                var name = prop.PropertyInfo.Name;
        //                //Translation
        //                result.Add(new ValidationResult(a.FormatErrorMessage("[" + name + "]"), new[] { prop.PropertyInfo.Name }));
        //            }
        //        }
        //    }

        //    return result;
        //}

        #endregion

        #region Clone?

        //???

        #endregion
    }

    public class DomainEntity<TKey> : DomainEntity, IKeyProvider<TKey>
    {
        public new TKey GetKey()
        {
            return (TKey)base.GetKey();
        }

        public void SetKey(TKey key)
        {
            //Assert New
        }
    }
}
