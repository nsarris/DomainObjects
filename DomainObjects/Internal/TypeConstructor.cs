using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Internal
{
    public class TypeConstructor<T>
    {
        private readonly Action<T> initializer;

        public TypeConstructor()
        {

        }

        public TypeConstructor(Action<T> initializer)
        {
            this.initializer = initializer;
        }

        protected ConstructorInfo GetConstructor(params Type[] types)
        {
            var ctor = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, types, null);
            if (ctor == null)
                throw new InvalidOperationException($"Type {typeof(T).Name} does not have a public constructor that with the specified argument");
            return ctor;
        }

        public T Construct(params object[] arguments)
        {
            return Construct(arguments.AsEnumerable());
        }

        public T Construct(IEnumerable<object> arguments)
        {
            var ctor = typeof(T).GetConstructorsEx().FirstOrDefault(c => c.Signature.Count == arguments.Count());
            if (ctor == null)
                throw new InvalidOperationException($"Type {typeof(T).Name} does not have a public constructor that has the specified number of arguments");

            var r = (T)ctor.Invoke(arguments);
            initializer?.Invoke(r);
            return r;
        }

        public T ConstructWithDefaults(params object[] arguments)
        {
            return ConstructWithDefaults(arguments.AsEnumerable());
        }

        public T ConstructWithDefaults(IEnumerable<object> arguments)
        {
            var ctor = typeof(T).GetConstructorsEx().FirstOrDefault(c => c.Signature.Count < arguments.Count());
            if (ctor == null)
                throw new InvalidOperationException($"Type {typeof(T).Name} does not have a public constructor that has that many arguments");

            var r = (T)ctor.InvokeWithDefaults(arguments);
            initializer?.Invoke(r);
            return r;
        }

        public T Construct(params (string name, object value)[] arguments)
        {
            return Construct(arguments.AsEnumerable());
        }

        public T Construct(IEnumerable<(string name, object value)> arguments)
        {
            var ctor = typeof(T).GetConstructorsEx().FirstOrDefault(c =>
                arguments.All(x => c.Signature.Keys.Contains(x.name)));

            if (ctor == null)
                throw new InvalidOperationException($"Type {typeof(T).Name} does not have a public constructor that has a constructor with given parameter names");

            var r = (T)ctor.Invoke(arguments);
            initializer?.Invoke(r);
            return r;
        }

        public T ConstructWithDefaults(params (string name, object value)[] arguments)
        {
            return ConstructWithDefaults(arguments.AsEnumerable());
        }

        public T ConstructWithDefaults(IEnumerable<(string name, object value)> arguments)
        {
            var ctor = typeof(T).GetConstructorsEx().FirstOrDefault(c =>
                arguments.All(x => c.Signature.Keys.Contains(x.name)));

            if (ctor == null)
                throw new InvalidOperationException($"Type {typeof(T).Name} does not have a public constructor that has a constructor with given parameter names");

            var r = (T)ctor.InvokeWithDefaults(arguments);
            initializer?.Invoke(r);
            return r;
        }
    }

    public class TypeConstructor<T, TArgument> : TypeConstructor<T>
    {
        private readonly Func<TArgument, T> constructor;

        public Type Type { get; }
        
        public TypeConstructor(Action<T> initializer)
            :base(initializer)
        {
            Type = typeof(T);
            constructor = MemberAccessorDelegateBuilder.CachedConstructorBuilder.Build<TArgument, T>(GetConstructor(typeof(TArgument)));
        }

        public TypeConstructor()
            :this(null)
        {
            
        }

        public T Construct(TArgument argument)
        {
            return constructor(argument);
        }
    }

    public class TypeConstructor<T, TArgument1, TArgument2> : TypeConstructor<T>
    {
        private readonly Func<TArgument1, TArgument2, T> constructor;

        public Type Type { get; }

        public TypeConstructor(Action<T> initializer)
            : base(initializer)
        {
            Type = typeof(T);
            constructor = MemberAccessorDelegateBuilder.CachedConstructorBuilder.Build<TArgument1, TArgument2, T>(GetConstructor(typeof(TArgument1), typeof(TArgument2)));
        }

        public TypeConstructor()
            : this(null)
        {

        }

        public T Construct(TArgument1 argument1, TArgument2 argument2)
        {
            return constructor(argument1, argument2);
        }
    }

    public class TypeConstructor<T, TArgument1, TArgument2, TArgument3> : TypeConstructor<T>
    {
        private readonly Func<TArgument1, TArgument2, TArgument3, T> constructor;

        public Type Type { get; }

        public TypeConstructor(Action<T> initializer)
            : base(initializer)
        {
            Type = typeof(T);
            constructor = MemberAccessorDelegateBuilder.CachedConstructorBuilder.Build<TArgument1, TArgument2, TArgument3, T>(GetConstructor(typeof(TArgument1), typeof(TArgument2), typeof(TArgument3)));
        }

        public TypeConstructor()
            : this(null)
        {

        }

        public T Construct(TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            return constructor(argument1, argument2, argument3);
        }
    }
}
