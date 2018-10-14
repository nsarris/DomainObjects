using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DomainObjects.ChangeTracking;
using DomainObjects.Metadata;
using DomainObjects.Validation;
using DomainObjects.Serialization;
using Dynamix.Reflection;

namespace DomainObjects.Core
{
    #pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public abstract class DomainEntity : DomainObject, IKeyProvider, ITrackableObject
    {
        #region Model Metadata

        [NonSerialized]
        private DomainEntityMetadata entityMetadata;

        public DomainEntityMetadata GetEntityMetadata()
        {
            if (entityMetadata == null)
                entityMetadata = DomainModelMetadataRegistry.GetEntityMetadta(this.GetType());

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

        protected DomainEntity(SerializationInfo info, StreamingContext context)
            : this()
        {
            Deserialize(info, context);
            //TODO: Try get, handle changetracking state
            SetObjectState(info.GetValue<DomainObjectState>("_state_"));
        }

        protected override void Serialize(SerializationInfo info, StreamingContext context)
        {
            base.Serialize(info, context);
            //if serialize state
            info.AddValue("_state_", GetObjectState());
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

        private void Init(bool isNew)
        {
            this.keyIsAssigned = this.GetEntityMetadata().GetKeyProperties().All(x => !Equals(x.Property.Get(this), x.Property.Type.DefaultOf()));

            if (isNew)
            {
                MarkNew();
            }
            else
            {
                if (!this.keyIsAssigned)
                    throw new InvalidOperationException($"Entity of type {this.GetType().Name} cannot be marked as existing when all of the key property values are default");

                MarkExisting();
            }

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

        protected void SetObjectState(DomainObjectState state)
        {
            this.state = state;
        }

        #endregion

        #region Change Tracking

        [NonSerialized]
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
    }

    public abstract class DomainEntity<T> : DomainEntity where T : DomainEntity<T>
    {
        protected DomainEntity()
        {

        }
        protected DomainEntity(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        #region Validation

        public virtual DomainValidationResult Validate()
        {
            return Validate(null);
        }

        public virtual DomainValidationResult Validate(IDomainValidator<T> validator)
        {
            return DomainValidationResult.Success;
        }

        #endregion

        #region Clone?

        //T Clone()???

        #endregion
    }

    public abstract class DomainEntity<T, TKey> : DomainEntity<T>, IKeyProvider<TKey>
        where T : DomainEntity<T>
    {
        protected DomainEntity()
        {

        }
        protected DomainEntity(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

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

        public bool KeyEquals(T other)
        {
            return this.GetKey() == other.GetKey();
        }

        public bool KeyEquals(DomainKey<TKey> other)
        {
            return this.GetKey() == other;
        }
    }
    #pragma warning restore S3925 // "ISerializable" should be implemented correctly
}
