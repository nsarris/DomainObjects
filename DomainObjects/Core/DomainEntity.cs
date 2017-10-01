using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamix;

namespace DomainObjects.Core
{
    public enum DomainObjectState
    {
        New,
        Existing,
        Deleted
    }

    public class DomainKeyAttribute : Attribute
    {
        public int Index { get; private set; }
        public DomainKeyAttribute(int index)
        {
            this.Index = index;
        }
    }

    public abstract class DomainEntity : DomainObject
    {
        #region Key and Equality
        public object GetKey()
        {
            return DomainEntityKeyProvider.GetKey(this);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (this.GetType() != obj.GetType())
                return false;

            return GetKey().Equals(((DomainEntity)obj).GetKey());
        }

        public override int GetHashCode()
        {
            return GetKey().GetHashCode();
        }

        public static bool operator ==(DomainEntity x, DomainEntity y)
        {
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
                return false;

            return x.Equals(y);
        }

        public static bool operator !=(DomainEntity x, DomainEntity y)
        {
            if (x == null || y == null)
                return false;

            return !x.Equals(y);
        }

        #endregion

        #region Change Tracking

        //object initvals;
        //public bool HasChanges { get; private set; }
        //List<string> changedProps = new List<string>();
        //public List<string> ChangedProperties { get { return changedProps.ToList(); } }
        //public void BeginTrackChanges()
        //{
        //    initvals = Activator.CreateInstance(this.GetType());
        //    this.PropertyChanged += ViewModelBase_PropertyChanged;
        //    foreach (var prop in this.GetType().GetProperties()
        //        .Where(x => x.GetSetMethod() != null))
        //        prop.SetValue(initvals, prop.GetValue(this));
        //}

        //void ViewModelBase_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (initvals != null)
        //    {
        //        var initval = this.GetType().GetProperty(e.PropertyName).GetValue(initvals);
        //        var currentval = this.GetType().GetProperty(e.PropertyName).GetValue(this);

        //        if ((initval == null && currentval != null)
        //            || (initval == null && currentval != null))
        //        {
        //            changedProps.Add(e.PropertyName);
        //            HasChanges = true;
        //        }
        //        else if (initval != null && currentval != null && !initval.Equals(currentval))
        //        {
        //            changedProps.Add(e.PropertyName);
        //            HasChanges = true;
        //        }
        //        else
        //        {
        //            changedProps.RemoveAll(x => x == e.PropertyName);
        //        }

        //        if (changedProps.Count == 0)
        //            HasChanges = false;
        //    }
        //}


        //public void CancelChanges()
        //{
        //    if (initvals != null)
        //    {
        //        foreach (var prop in this.GetType().GetProperties()
        //            .Where(x => x.GetSetMethod() != null))
        //            prop.SetValue(this, prop.GetValue(initvals));
        //        initvals = null;
        //        this.PropertyChanged += ViewModelBase_PropertyChanged;
        //    }
        //}

        //public void AcceptChanges()
        //{
        //    if (initvals != null)
        //    {
        //        initvals = null;
        //        this.PropertyChanged += ViewModelBase_PropertyChanged;
        //    }
        //}

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
}
