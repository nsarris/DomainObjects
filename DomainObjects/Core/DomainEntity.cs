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
        }

        public bool KeyEquals(DomainEntity other)
        {
            if (other == null || other.GetType() != this.GetType())
                return false;

            return GetKey().Equals(other.GetKey());
        }

        public void SetKey(params object[] values)
        {
            AssertSetKey();

            GetEntityMetadata().SetKey(this, values);
        }

        public void SetKey(object value)
        {
            AssertSetKey();

            GetEntityMetadata().SetKey(this, value);
        }

        protected void AssertSetKey()
        {
            if (this.state != DomainObjectState.New)
                throw new InvalidOperationException("Entity keys can only be set after initialization on entities in New state");
        }

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

            BeginTrackingDeep();
        }

        public void InitNew() => Init(true);
        public void InitExisting() => Init(false);

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
            return changeTracker.GetIsChangedDeep();
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

        public void ResetChangesDeep()
        {
            changeTracker.ResetChangesDeep();
        }

        public void AcceptChangesDeep()
        {
            changeTracker.AcceptChangesDeep();
        }

        public void MarkChangedDeep()
        {
            changeTracker.MarkChangedDeep();
        }

        public void MarkUnchangedDeep()
        {
            changeTracker.MarkUnchangedDeep();
        }

        public void BeginTrackingDeep()
        {
            changeTracker.BeginTrackingDeep();
        }

        public void StopTrackingDeep()
        {
            changeTracker.StopTrackingDeep();
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
            base.SetKey(key);
        }
    }
}
