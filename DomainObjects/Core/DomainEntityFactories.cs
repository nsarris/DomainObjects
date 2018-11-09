using System;
using DomainObjects.Internal;
using Dynamix.Reflection;

namespace DomainObjects.Core
{
	public class DomainEntityFactory<T, TArgs1, TArgs2> 
		: TypeConstructor<T, TArgs1, TArgs2>
        where T : DomainEntity
    {
        public TypeConstructor<T, TArgs1, TArgs2> New { get; }
        public TypeConstructor<T, TArgs1, TArgs2> Existing { get; }

        public DomainEntityFactory(Action<T> initializer = null)
        {
            var concreteType = ProxyTypeBuilder.BuildPropertyChangedProxy<T>();
            New = new TypeConstructor<T, TArgs1, TArgs2>(concreteType, x => { initializer?.Invoke(x); x.OnCreated(); });
            Existing = new TypeConstructor<T, TArgs1, TArgs2>(concreteType, x => { initializer?.Invoke(x); x.OnLoaded(); });
        }
    }

	public class DomainEntityFactory<T, TArgs1, TArgs2, TArgs3> 
		: TypeConstructor<T, TArgs1, TArgs2, TArgs3>
        where T : DomainEntity
    {
        public TypeConstructor<T, TArgs1, TArgs2, TArgs3> New { get; }
        public TypeConstructor<T, TArgs1, TArgs2, TArgs3> Existing { get; }

        public DomainEntityFactory(Action<T> initializer = null)
        {
            var concreteType = ProxyTypeBuilder.BuildPropertyChangedProxy<T>();
            New = new TypeConstructor<T, TArgs1, TArgs2, TArgs3>(concreteType, x => { initializer?.Invoke(x); x.OnCreated(); });
            Existing = new TypeConstructor<T, TArgs1, TArgs2, TArgs3>(concreteType, x => { initializer?.Invoke(x); x.OnLoaded(); });
        }
    }

	public class DomainEntityFactory<T, TArgs1, TArgs2, TArgs3, TArgs4> 
		: TypeConstructor<T, TArgs1, TArgs2, TArgs3, TArgs4>
        where T : DomainEntity
    {
        public TypeConstructor<T, TArgs1, TArgs2, TArgs3, TArgs4> New { get; }
        public TypeConstructor<T, TArgs1, TArgs2, TArgs3, TArgs4> Existing { get; }

        public DomainEntityFactory(Action<T> initializer = null)
        {
            var concreteType = ProxyTypeBuilder.BuildPropertyChangedProxy<T>();
            New = new TypeConstructor<T, TArgs1, TArgs2, TArgs3, TArgs4>(concreteType, x => { initializer?.Invoke(x); x.OnCreated(); });
            Existing = new TypeConstructor<T, TArgs1, TArgs2, TArgs3, TArgs4>(concreteType, x => { initializer?.Invoke(x); x.OnLoaded(); });
        }
    }

}