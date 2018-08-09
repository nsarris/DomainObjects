using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Core
{
    public class Trackable //: INotifyPropertyChanged
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        bool suppressPropertyChanged;
        Dictionary<string, object> originalValues;
        Dictionary<string, object> changes = new Dictionary<string, object>();
        public Trackable()
        {
            originalValues = GetCurrentValues();
        }

        private Dictionary<string, object> GetCurrentValues()
        {
            return this.GetType()
                .GetPropertiesEx(BindingFlagsEx.All)
                .Where(x => x.CanGet && x.CanSet)
                .ToDictionary(x => x.Name, x => x.Get(this));
        }

        private void SetOriginalValues()
        {
            foreach (var prop in this.GetType()
                    .GetPropertiesEx(BindingFlagsEx.All)
                    .Where(x => x.CanGet && x.CanSet))
                prop.Set(this, originalValues[prop.Name]);
        }

        public void ResetChanges()
        {
            suppressPropertyChanged = true;
            try
            {
                SetOriginalValues();
                changes = new Dictionary<string, object>();
                IsChanged = false;
            }
            finally
            {
                suppressPropertyChanged = false;
            }
        }

        public void AcceptChanges()
        {
            originalValues = GetCurrentValues();
            changes = new Dictionary<string, object>();
            IsChanged = false;
        }

        public bool IsChanged { get; private set; }

        protected void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (suppressPropertyChanged)
                return;

            if (!object.Equals(after, originalValues[propertyName]))
            {
                IsChanged = true;
                changes[propertyName] = after;
            }
            else
            {
                IsChanged = false;
                changes = new Dictionary<string, object>();
                foreach (var prop in this.GetType().GetPropertiesExDic(BindingFlagsEx.All))
                {
                    var v = prop.Value.Get(this);
                    if (!object.Equals(v, originalValues[prop.Key]))
                    {
                        IsChanged = true;
                        changes[prop.Key] = v;
                    }
                }
            }
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
