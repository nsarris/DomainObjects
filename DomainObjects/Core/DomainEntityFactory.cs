using DomainObjects.Internal;
using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Core
{
    class DomainEntityFactory<T, TArgs>
        where T : DomainEntity
    {
        public TypeConstructor<T, TArgs> New { get; }
        public TypeConstructor<T, TArgs> Existing { get; }

        public DomainEntityFactory(Action<T> initializer = null)
        {
            var concreteType = ProxyTypeBuilder.BuildPropertyChangedProxy<T>();
            New = new TypeConstructor<T, TArgs>(concreteType, x => { initializer?.Invoke(x); x.InitNew(); });
            Existing = new TypeConstructor<T, TArgs>(concreteType, x => { initializer?.Invoke(x); x.InitExisting(); });
        }
    }

    class DomainEntityFactory<T, TArgs1, TArgs2> : TypeConstructor<T, TArgs1, TArgs2>
        where T : DomainEntity
    {
        public TypeConstructor<T, TArgs1, TArgs2> New { get; }
        public TypeConstructor<T, TArgs1, TArgs2> Existing { get; }

        public DomainEntityFactory(Action<T> initializer = null)
        {
            var concreteType = ProxyTypeBuilder.BuildPropertyChangedProxy<T>();
            New = new TypeConstructor<T, TArgs1, TArgs2>(concreteType, x => { initializer?.Invoke(x); x.InitNew(); });
            Existing = new TypeConstructor<T, TArgs1, TArgs2>(concreteType, x => { initializer?.Invoke(x); x.InitExisting(); });
        }
    }

    class DomainEntityFactory<T, TArgs1, TArgs2, TArgs3> : TypeConstructor<T, TArgs1, TArgs2, TArgs3>
        where T : DomainEntity
    {
        public TypeConstructor<T, TArgs1, TArgs2, TArgs3> New { get; }
        public TypeConstructor<T, TArgs1, TArgs2, TArgs3> Existing { get; }

        public DomainEntityFactory(Action<T> initializer = null)
        {
            var concreteType = ProxyTypeBuilder.BuildPropertyChangedProxy<T>();
            New = new TypeConstructor<T, TArgs1, TArgs2, TArgs3>(concreteType, x => { initializer?.Invoke(x); x.InitNew(); });
            Existing = new TypeConstructor<T, TArgs1, TArgs2, TArgs3>(concreteType, x => { initializer?.Invoke(x); x.InitExisting(); });
        }
    }

    class DomainEntityFactory<T, TArgs1, TArgs2, TArgs3, TArgs4> : TypeConstructor<T, TArgs1, TArgs2, TArgs3, TArgs4>
        where T : DomainEntity
    {
        public TypeConstructor<T, TArgs1, TArgs2, TArgs3, TArgs4> New { get; }
        public TypeConstructor<T, TArgs1, TArgs2, TArgs3, TArgs4> Existing { get; }

        public DomainEntityFactory(Action<T> initializer = null)
        {
            var concreteType = ProxyTypeBuilder.BuildPropertyChangedProxy<T>();
            New = new TypeConstructor<T, TArgs1, TArgs2, TArgs3, TArgs4>(concreteType, x => { initializer?.Invoke(x); x.InitNew(); });
            Existing = new TypeConstructor<T, TArgs1, TArgs2, TArgs3, TArgs4>(concreteType, x => { initializer?.Invoke(x); x.InitExisting(); });
        }
    }
}
