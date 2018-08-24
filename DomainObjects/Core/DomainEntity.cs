using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainObjects.ChangeTracking;
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

        private readonly UnsetKey unsetKey = new UnsetKey();
        private bool keyIsSet;

        public bool GetKeyIsSet() => keyIsSet;
        internal UnsetKey GetUnSetKey() => unsetKey;

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

            keyIsSet = true;
        }

        public void SetKey(object value)
        {
            AssertSetKey();

            GetEntityMetadata().SetKey(this, value);

            keyIsSet = true;
        }

        protected void AssertSetKey()
        {
            if (keyIsSet)
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
                this.keyIsSet = keyIsSet;
            }
            else
            {
                MarkExisting();
                this.keyIsSet = true;
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
