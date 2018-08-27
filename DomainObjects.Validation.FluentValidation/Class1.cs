using Dynamix.Reflection;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Validation.FluentValidation
{
    public interface IDomainValidator
    {
        global::FluentValidation.IValidator FluentValidator { get; }

        TDependency GetDependency<TDependency>();
        object GetDependency(Type dependencyType);
        TDependency GetDependency<TDependency>(string name);
        object GetDependency(Type dependencyType, string name);
    }

    public interface IDomainValidator<T> : IDomainValidator
    {
        new AbstractValidator<T> FluentValidator { get; }
    }

    public class DomainValidator<T> : IDomainValidator<T>
    {
        public DomainValidator(AbstractValidator<T> fluentValidator)
        {
            FluentValidator = fluentValidator;
        }

        protected void AutoRegisterDependencies()
        {
            //Register all properties as dependencies
            //Use Attributes to configure
        }

        private Dictionary<(Type,string), object> Dependencies { get; } = new Dictionary<(Type, string), object>();
        public AbstractValidator<T> FluentValidator { get; }

        IValidator IDomainValidator.FluentValidator => FluentValidator;

        public ValidationResult Validate(T instance)
        {
            var context = new RootValidationContext<T>(instance, new TestValidatorResolver());

            //TODO: Use attribute to insert in root context data with name
            //Note: FluentValidation specific
            //foreach (var item in Dependencies)
              //  context.RootContextData.Add(item.Key.Name, item.Value);
            return FluentValidator.Validate(context);
        }


        protected void RegisterDependency(object dependency)
        {
            //AssertNotNull
            Dependencies[(dependency.GetType(), null)] = dependency;
        }
        protected void RegisterDependency(object dependency, string name)
        {
            //AssertNotNull
            Dependencies[(dependency.GetType(), name)] = dependency;
        }

        protected void RegisterDependency<TDepedency>(object dependency)
        {
            //check type matches
            Dependencies[(typeof(TDepedency), null)] = dependency;
        }
        protected void RegisterDependency<TDepedency>(object dependency, string name)
        {
            //check type matches
            Dependencies[(typeof(TDepedency), name)] = dependency;
        }

        protected void RegisterDependency(Type registerAs, object dependency)
        {
            //check type matches
            Dependencies[(registerAs, null)] = dependency;
        }
        protected void RegisterDependency(Type registerAs, object dependency, string name)
        {
            //check type matches
            Dependencies[(registerAs, name)] = dependency;
        }


        public TDependency GetDependency<TDependency>()
        {
            return (TDependency)Dependencies[(typeof(TDependency), null)];
        }

        

        public object GetDependency(Type dependencyType)
        {
            return Dependencies[(dependencyType, null)];
        }
        public TDependency GetDependency<TDependency>(string name)
        {
            return (TDependency)Dependencies[(typeof(TDependency), name)];
        }

        public object GetDependency(Type dependencyType, string name)
        {
            return Dependencies[(dependencyType, name)];
        }
    }

    public enum ChildContextType
    {
        None,
        Child,
        ChildCollection
    }

    public interface ICustomValidationContext : IValidationContext
    {
        ICustomValidationContext RootContext { get; }
        new ICustomValidationContext ParentContext { get; }
        IValidatorResolver ValidatorResolver { get; }
    }

    public class CustomValidationContext<T> : ValidationContext<T>, ICustomValidationContext
    {
        public ICustomValidationContext RootContext { get; }
        public ICustomValidationContext ParentContext { get; }

        //Make this private or protected
        public IValidatorResolver ValidatorResolver { get; }
        //Hide Resolver and expose a Function to intercept resolution. 
        //Register each resolved validator/dependencies in resolvedValidator
        //Implement to GetDependency methods. 
        //Scan all validators from bottom to top for dependency

        private readonly Dictionary<Type, IDomainValidator> resolvedValidators = new Dictionary<Type, IDomainValidator>();

        private readonly ChildContextType childContextType = ChildContextType.None;
        public CustomValidationContext(T instanceToValidate, IValidatorResolver validatorResolver) : base(instanceToValidate)
        {
            RootContext = this;
            ValidatorResolver = validatorResolver;
        }

        public CustomValidationContext(T instanceToValidate, PropertyChain propertyChain, IValidatorSelector validatorSelector, IValidatorResolver validatorResolver)
            : base(instanceToValidate, propertyChain, validatorSelector)
        {
            RootContext = this;
            ValidatorResolver = validatorResolver;
        }

        public CustomValidationContext(T instanceToValidate, PropertyChain propertyChain, IValidatorSelector validatorSelector, ChildContextType childContextType, ICustomValidationContext parentValidationContext) 
            : base(instanceToValidate, propertyChain, validatorSelector)
        {
            this.childContextType = childContextType;
            ParentContext = parentValidationContext;
            RootContext = parentValidationContext.RootContext;
            ValidatorResolver = parentValidationContext.RootContext.ValidatorResolver;
        }

        public CustomValidationContext<TChild> CloneForChildValidator<TChild>(TChild child)
        {
            var clone = new CustomValidationContext<TChild>(child, this.PropertyChain, this.Selector, ChildContextType.Child, this.RootContext);
            foreach (var data in RootContextData)
                clone.RootContextData.Add(data);
            return clone;
        }

        public CustomValidationContext<TChild> CloneForChildCollectionValidator<TChild>(TChild child)
        {
            var clone = new CustomValidationContext<TChild>(child, this.PropertyChain, this.Selector, ChildContextType.ChildCollection, this.RootContext);
            foreach (var data in RootContextData)
                clone.RootContextData.Add(data);
            return clone;
        }

        public TValidator ResolveChildValidator<TValidator>() where TValidator : IDomainValidator
        {
            if (resolvedValidators.TryGetValue(typeof(TValidator), out var validator))
                return (TValidator)validator;

            var resolvedValidator = ValidatorResolver.ResolveValidator<TValidator>();
            resolvedValidators[typeof(TValidator)] = resolvedValidator;
            return resolvedValidator;
        }

        //ResolveForChild

        public override bool IsChildContext => childContextType == ChildContextType.Child;
        public override bool IsChildCollectionContext => childContextType == ChildContextType.ChildCollection;
    }

    public interface IValidatorResolver
    {
        DomainValidator<T> ResolveValidatorFor<T>();
        T ResolveValidator<T>() where T : IDomainValidator;
    }

    public class TestValidatorResolver : IValidatorResolver
    {
        public T ResolveValidator<T>() where T : IDomainValidator
        {
            //return Activator.CreateInstance<T>();
            return (T)(typeof(T).GetConstructorsEx().First().InvokeWithDefaults());
        }

        public DomainValidator<T> ResolveValidatorFor<T>()
        {
            throw new NotImplementedException();
        }
    }

    public interface IRootValidationContext
    {
        IValidatorResolver ValidatorResolver { get; }
    }

    public class RootValidationContext<T> : CustomValidationContext<T>, IRootValidationContext
    {
        public RootValidationContext(T instanceToValidate, IValidatorResolver validatorResolver) : base(instanceToValidate, validatorResolver)
        {
        }
    }
}
