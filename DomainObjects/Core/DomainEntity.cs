using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainObjects.ChangeTracking;
using DomainObjects.Metadata;
using DomainObjects.Validation;

namespace DomainObjects.Core
{
    public abstract class DomainEntity : DomainObject, IKeyProvider, ITrackable
    {
        #region Model Metadata

        private DomainEntityMetadata entityMetadata;

        public DomainEntityMetadata GetEntityMetadata()
        {
            if (entityMetadata == null)
                entityMetadata = DomainModelMetadataRegistry.GetEntityDescriptor(this.GetType());

            return entityMetadata;
        }

        #endregion

        #region Ctor

        protected DomainEntity()
        {
            changeTracker = new ChangeTracker(this);
            changeTracker.BeforePropertyChanged += (object sender, PropertyChangedExtendedEventArgs e) => OnBeforePropertyChanged(e.PropertyName, e.Before, e.After);
            changeTracker.AfterPropertyChanged += (object sender, PropertyChangedExtendedEventArgs e) => OnAfterPropertyChanged(e.PropertyName, e.Before, e.After);
        }

        #endregion

        #region Key and Equality

        private readonly UnassignedKey unassignedKey = new UnassignedKey();
        private bool keyIsAssigned;

        public bool GetKeyIsAssigned() => keyIsAssigned;
        internal UnassignedKey GetUnAssignedKey() => unassignedKey;

        public IDomainKey GetKey()
        {
            return (IDomainKey)GetEntityMetadata().GetKey(this);
        }

        public object GetKeyValue()
        {
            return GetEntityMetadata().GetKeyValue(this);
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

            keyIsAssigned = true;
        }

        public void SetKey(object value)
        {
            AssertSetKey();

            GetEntityMetadata().SetKey(this, value);

            keyIsAssigned = true;
        }

        protected void AssertSetKey()
        {
            if (keyIsAssigned)
                throw new InvalidOperationException($"Key has already been marked as set for entity {this.GetType().Name}");
        }

        #endregion

        #region ObjectState

        private DomainObjectState state = DomainObjectState.Uninitialized;

        public DomainObjectState GetObjectState()
        {
            return state;
        }

        private void Init(bool isNew, bool keyIsSet)
        {
            if (isNew)
            {
                MarkNew();
                this.keyIsAssigned = keyIsSet;
            }
            else
            {
                MarkExisting();
                this.keyIsAssigned = true;
            }

            BeginTrackingDeep();
        }

        public void InitNew(bool keyIsSet) => Init(true, keyIsSet);
        public void InitExisting() => Init(false, true);

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

        public virtual DomainValidationResult Validate()
        {
            Validate(null);
            return null;
        }

        public virtual DomainValidationResult Validate(IDomainValidator<Task> validationContext)
        {
            return DomainValidationResult.Success;
        }

        #endregion

        #region Clone?

        //???

        #endregion
    }

    public class DomainEntity<TKey> : DomainEntity, IKeyProvider<TKey>
    {
        public new DomainKey<TKey> GetKey()
        {
            return (DomainKey<TKey>)base.GetKey();
        }

        public void SetKey(TKey key)
        {
            base.SetKey(key);
        }

        public new TKey GetKeyValue()
        {
            return (TKey)base.GetKeyValue();
        }
    }
}
