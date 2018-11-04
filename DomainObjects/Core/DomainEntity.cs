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
        private Lazy<DomainEntityMetadata> entityMetadata;

        public DomainEntityMetadata GetEntityMetadata()
        {
            return entityMetadata.Value;
        }

        #endregion

        #region Ctor

        protected DomainEntity()
        {
            entityMetadata = new Lazy<DomainEntityMetadata>(() => DomainModelMetadataRegistry.GetEntityMetadta(this.GetType()));

            ChangeTracker = new ChangeTracker(this);
            ChangeTracker.BeforePropertyChanged += (object sender, PropertyChangedExtendedEventArgs e) => OnBeforePropertyChanged(e.PropertyName, e.Before, e.After);
            ChangeTracker.AfterPropertyChanged += (object sender, PropertyChangedExtendedEventArgs e) => OnAfterPropertyChanged(e.PropertyName, e.Before, e.After);
        }

        protected DomainEntity(SerializationInfo info, StreamingContext context)
            : this()
        {
            Deserialize(info, context);
            //TODO: Try get, handle changetracking state
        }

        #endregion

        #region Key and Equality

        private readonly UnassignedKey unassignedKey = new UnassignedKey();
        private bool keyIsAssigned;

        public bool GetKeyIsAssigned() => keyIsAssigned;
        internal UnassignedKey GetUnassignedKey() => unassignedKey;

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

        #region EntityState

        private EntityState entityState = EntityState.Uninitialized;

        public EntityState GetEntityState()
        {
            return entityState;
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

            ChangeTracker.BeginTrackingDeep();
        }

        public void InitNew() => Init(true);
        public void InitExisting() => Init(false);

        public void MarkNew()
        {
            entityState = EntityState.New;
        }

        public void MarkExisting()
        {
            entityState = EntityState.Existing;
        }

        public void MarkDeleted()
        {
            entityState = EntityState.Deleted;
        }

        protected void SetObjectState(EntityState state)
        {
            this.entityState = state;
        }

        #endregion

        #region Change Tracking

        [field:NonSerialized]
        public ChangeTracker ChangeTracker { get; }

        void ITrackable.MarkChanged()
        {
            ChangeTracker.MarkChanged();
        }

        void ITrackable.MarkUnchanged()
        {
            ChangeTracker.MarkUnchanged();
        }

        bool ITrackable.GetIsChanged()
        {
            return ChangeTracker.GetIsChanged();
        }

        bool ITrackable.GetIsChangedDeep()
        {
            return ChangeTracker.GetIsChangedDeep();
        }


        void ITrackable.ResetChanges()
        {
            ChangeTracker.ResetChanges();
        }

        void ITrackable.AcceptChanges()
        {
            ChangeTracker.AcceptChanges();
        }

        IReadOnlyDictionary<string, object> ITrackableObject.GetChanges()
        {
            return ChangeTracker.GetChanges();
        }

        void ITrackable.BeginTracking()
        {
            ChangeTracker.BeginTracking();
        }

        void ITrackable.StopTracking()
        {
            ChangeTracker.StopTracking();
        }

        protected void OnPropertyChanged(string propertyName, object before, object after)
        {
            ChangeTracker.OnPropertyChanged(propertyName, before, after);
        }

        protected virtual void OnBeforePropertyChanged(string propertyName, object before, object after)
        {

        }

        protected virtual void OnAfterPropertyChanged(string propertyName, object before, object after)
        {

        }

        void ITrackable.ResetChangesDeep()
        {
            ChangeTracker.ResetChangesDeep();
        }

        void ITrackable.AcceptChangesDeep()
        {
            ChangeTracker.AcceptChangesDeep();
        }

        void ITrackable.MarkChangedDeep()
        {
            ChangeTracker.MarkChangedDeep();
        }

        void ITrackable.MarkUnchangedDeep()
        {
            ChangeTracker.MarkUnchangedDeep();
        }

        void ITrackable.BeginTrackingDeep()
        {
            ChangeTracker.BeginTrackingDeep();
        }

        void ITrackable.StopTrackingDeep()
        {
            ChangeTracker.StopTrackingDeep();
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

        //TODO
        //T Clone() 

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
