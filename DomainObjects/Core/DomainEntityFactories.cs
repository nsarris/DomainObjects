using System;
using DomainObjects.Internal;
using Dynamix.Reflection;

namespace DomainObjects.Core
{
	public class DomainEntityFactory<T, TArg1, TArg2> 
		: TypeConstructor<T, TArg1, TArg2>
        where T : DomainEntity
    {
        public TypeConstructor<T, TArg1, TArg2> New { get; }
        public TypeConstructor<T, TArg1, TArg2> Existing { get; }

        public DomainEntityFactory(Action<T> initializer = null)
        {
            var concreteType = ProxyTypeBuilder.BuildPropertyChangedProxy<T>();
            New = new TypeConstructor<T, TArg1, TArg2>(concreteType, x => { initializer?.Invoke(x); x.OnCreated(); });
            Existing = new TypeConstructor<T, TArg1, TArg2>(concreteType, x => { initializer?.Invoke(x); x.OnLoaded(); });
        }
    }

	public class DomainEntityFactory<T, TArg1, TArg2, TArg3> 
		: TypeConstructor<T, TArg1, TArg2, TArg3>
        where T : DomainEntity
    {
        public TypeConstructor<T, TArg1, TArg2, TArg3> New { get; }
        public TypeConstructor<T, TArg1, TArg2, TArg3> Existing { get; }

        public DomainEntityFactory(Action<T> initializer = null)
        {
            var concreteType = ProxyTypeBuilder.BuildPropertyChangedProxy<T>();
            New = new TypeConstructor<T, TArg1, TArg2, TArg3>(concreteType, x => { initializer?.Invoke(x); x.OnCreated(); });
            Existing = new TypeConstructor<T, TArg1, TArg2, TArg3>(concreteType, x => { initializer?.Invoke(x); x.OnLoaded(); });
        }
    }

	public class DomainEntityFactory<T, TArg1, TArg2, TArg3, TArg4> 
		: TypeConstructor<T, TArg1, TArg2, TArg3, TArg4>
        where T : DomainEntity
    {
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4> New { get; }
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4> Existing { get; }

        public DomainEntityFactory(Action<T> initializer = null)
        {
            var concreteType = ProxyTypeBuilder.BuildPropertyChangedProxy<T>();
            New = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4>(concreteType, x => { initializer?.Invoke(x); x.OnCreated(); });
            Existing = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4>(concreteType, x => { initializer?.Invoke(x); x.OnLoaded(); });
        }
    }

	public class DomainEntityFactory<T, TArg1, TArg2, TArg3, TArg4, TArg5> 
		: TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5>
        where T : DomainEntity
    {
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5> New { get; }
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5> Existing { get; }

        public DomainEntityFactory(Action<T> initializer = null)
        {
            var concreteType = ProxyTypeBuilder.BuildPropertyChangedProxy<T>();
            New = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5>(concreteType, x => { initializer?.Invoke(x); x.OnCreated(); });
            Existing = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5>(concreteType, x => { initializer?.Invoke(x); x.OnLoaded(); });
        }
    }

	public class DomainEntityFactory<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> 
		: TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>
        where T : DomainEntity
    {
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> New { get; }
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> Existing { get; }

        public DomainEntityFactory(Action<T> initializer = null)
        {
            var concreteType = ProxyTypeBuilder.BuildPropertyChangedProxy<T>();
            New = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(concreteType, x => { initializer?.Invoke(x); x.OnCreated(); });
            Existing = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(concreteType, x => { initializer?.Invoke(x); x.OnLoaded(); });
        }
    }

	public class DomainEntityFactory<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> 
		: TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>
        where T : DomainEntity
    {
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> New { get; }
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> Existing { get; }

        public DomainEntityFactory(Action<T> initializer = null)
        {
            var concreteType = ProxyTypeBuilder.BuildPropertyChangedProxy<T>();
            New = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(concreteType, x => { initializer?.Invoke(x); x.OnCreated(); });
            Existing = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(concreteType, x => { initializer?.Invoke(x); x.OnLoaded(); });
        }
    }

	public class DomainEntityFactory<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> 
		: TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>
        where T : DomainEntity
    {
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> New { get; }
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> Existing { get; }

        public DomainEntityFactory(Action<T> initializer = null)
        {
            var concreteType = ProxyTypeBuilder.BuildPropertyChangedProxy<T>();
            New = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(concreteType, x => { initializer?.Invoke(x); x.OnCreated(); });
            Existing = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(concreteType, x => { initializer?.Invoke(x); x.OnLoaded(); });
        }
    }

	public class DomainEntityFactory<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> 
		: TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>
        where T : DomainEntity
    {
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> New { get; }
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> Existing { get; }

        public DomainEntityFactory(Action<T> initializer = null)
        {
            var concreteType = ProxyTypeBuilder.BuildPropertyChangedProxy<T>();
            New = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(concreteType, x => { initializer?.Invoke(x); x.OnCreated(); });
            Existing = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(concreteType, x => { initializer?.Invoke(x); x.OnLoaded(); });
        }
    }

	public class DomainEntityFactory<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10> 
		: TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>
        where T : DomainEntity
    {
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10> New { get; }
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10> Existing { get; }

        public DomainEntityFactory(Action<T> initializer = null)
        {
            var concreteType = ProxyTypeBuilder.BuildPropertyChangedProxy<T>();
            New = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>(concreteType, x => { initializer?.Invoke(x); x.OnCreated(); });
            Existing = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>(concreteType, x => { initializer?.Invoke(x); x.OnLoaded(); });
        }
    }

	public class DomainEntityFactory<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11> 
		: TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11>
        where T : DomainEntity
    {
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11> New { get; }
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11> Existing { get; }

        public DomainEntityFactory(Action<T> initializer = null)
        {
            var concreteType = ProxyTypeBuilder.BuildPropertyChangedProxy<T>();
            New = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11>(concreteType, x => { initializer?.Invoke(x); x.OnCreated(); });
            Existing = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11>(concreteType, x => { initializer?.Invoke(x); x.OnLoaded(); });
        }
    }

	public class DomainEntityFactory<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12> 
		: TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12>
        where T : DomainEntity
    {
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12> New { get; }
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12> Existing { get; }

        public DomainEntityFactory(Action<T> initializer = null)
        {
            var concreteType = ProxyTypeBuilder.BuildPropertyChangedProxy<T>();
            New = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12>(concreteType, x => { initializer?.Invoke(x); x.OnCreated(); });
            Existing = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12>(concreteType, x => { initializer?.Invoke(x); x.OnLoaded(); });
        }
    }

	public class DomainEntityFactory<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13> 
		: TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13>
        where T : DomainEntity
    {
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13> New { get; }
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13> Existing { get; }

        public DomainEntityFactory(Action<T> initializer = null)
        {
            var concreteType = ProxyTypeBuilder.BuildPropertyChangedProxy<T>();
            New = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13>(concreteType, x => { initializer?.Invoke(x); x.OnCreated(); });
            Existing = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13>(concreteType, x => { initializer?.Invoke(x); x.OnLoaded(); });
        }
    }

	public class DomainEntityFactory<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14> 
		: TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14>
        where T : DomainEntity
    {
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14> New { get; }
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14> Existing { get; }

        public DomainEntityFactory(Action<T> initializer = null)
        {
            var concreteType = ProxyTypeBuilder.BuildPropertyChangedProxy<T>();
            New = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14>(concreteType, x => { initializer?.Invoke(x); x.OnCreated(); });
            Existing = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14>(concreteType, x => { initializer?.Invoke(x); x.OnLoaded(); });
        }
    }

	public class DomainEntityFactory<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15> 
		: TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15>
        where T : DomainEntity
    {
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15> New { get; }
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15> Existing { get; }

        public DomainEntityFactory(Action<T> initializer = null)
        {
            var concreteType = ProxyTypeBuilder.BuildPropertyChangedProxy<T>();
            New = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15>(concreteType, x => { initializer?.Invoke(x); x.OnCreated(); });
            Existing = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15>(concreteType, x => { initializer?.Invoke(x); x.OnLoaded(); });
        }
    }

	public class DomainEntityFactory<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16> 
		: TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16>
        where T : DomainEntity
    {
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16> New { get; }
        public TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16> Existing { get; }

        public DomainEntityFactory(Action<T> initializer = null)
        {
            var concreteType = ProxyTypeBuilder.BuildPropertyChangedProxy<T>();
            New = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16>(concreteType, x => { initializer?.Invoke(x); x.OnCreated(); });
            Existing = new TypeConstructor<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16>(concreteType, x => { initializer?.Invoke(x); x.OnLoaded(); });
        }
    }

}