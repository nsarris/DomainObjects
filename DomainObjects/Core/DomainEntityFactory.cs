using DomainObjects.Internal;
using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Core
{
    public class DomainEntityFactory<T, TArgs> : TypeConstructor<T, TArgs>
        where T : DomainEntity
    {
        public TypeConstructor<T, TArgs> New { get; }
        public TypeConstructor<T, TArgs> Existing { get; }

        public DomainEntityFactory(Action<T> initializer = null)
        {
            var concreteType = ProxyTypeBuilder.BuildPropertyChangedProxy<T>();
            New = new TypeConstructor<T, TArgs>(concreteType, x => { initializer?.Invoke(x); x.OnCreated(); });
            Existing = new TypeConstructor<T, TArgs>(concreteType, x => { initializer?.Invoke(x); x.OnLoaded(); });
        }
    }
}
