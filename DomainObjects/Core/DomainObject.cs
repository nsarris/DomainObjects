using Dynamix.Reflection;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Core
{
    public abstract class DomainObject//<T> where T : DomainObject<T>
    {
        public object this[string property]
        {
            get
            {
                return this.GetType().GetPropertyEx(property).Get(this);
            }
            set
            {
                var prop = this.GetType().GetPropertyEx(property, BindingFlagsEx.AllInstance);
                if (prop.PublicSet)
                    prop.Set(value);
                else
                    throw new InvalidOperationException($"Property {prop.Name} of {this.GetType().Name} does not have a public set method");
            }
        }
    }
}
