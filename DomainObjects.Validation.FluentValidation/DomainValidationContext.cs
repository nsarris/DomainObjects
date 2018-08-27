using FluentValidation;
using FluentValidation.Internal;
using System;
using System.Collections.Generic;

namespace DomainObjects.Validation.FluentValidation
{
    public class DomainValidationContext<T> : ValidationContext<T>, IDomainValidationContext
    {
        public IDomainValidationContext RootContext { get; }
        public IDomainValidationContext ParentContext { get; }
        public IDomainFluentValidator<T> Validator { get; }
        
        private readonly ChildContextType childContextType = ChildContextType.None;
        public DomainValidationContext(T instanceToValidate, IDomainFluentValidator<T> validator) : base(instanceToValidate)
        {
            RootContext = this;
            Validator = validator;
        }

        public DomainValidationContext(T instanceToValidate, PropertyChain propertyChain, IValidatorSelector validatorSelector, IDomainFluentValidator<T> validator)
            : base(instanceToValidate, propertyChain, validatorSelector)
        {
            RootContext = this;
            Validator = validator;
        }

        public DomainValidationContext(T instanceToValidate, PropertyChain propertyChain, IValidatorSelector validatorSelector, ChildContextType childContextType, IDomainFluentValidator<T> validator, IDomainValidationContext parentValidationContext)
            : base(instanceToValidate, propertyChain, validatorSelector)
        {
            this.childContextType = childContextType;
            ParentContext = parentValidationContext;
            RootContext = parentValidationContext.RootContext;
            Validator = validator;
        }

        public DomainValidationContext<TChild> CloneForChildValidator<TChild, TValidator>(TChild child)
            where TValidator : IDomainObjectFluentValidator<TChild>
        {
            return CloneForChildCollectionValidator(child, ResolveChildValidator<TValidator>());
        }

        public DomainValidationContext<TChild> CloneForChildCollectionValidator<TChild, TValidator>(TChild child)
            where TValidator : IDomainObjectFluentValidator<TChild>
        {
            return CloneForChildCollectionValidator(child, ResolveChildValidator<TValidator>());
        }

        public TValidator ResolveChildValidator<TValidator>() where TValidator : IDomainValidator
        {
            if (Validator is IDomainObjectValidator objectValidator)
                return objectValidator.ResolveChildValidator<TValidator>();
            else
                throw new InvalidOperationException("Primitive validators do not support resolving child validators");
        }

        public DomainValidationContext<TChild> CloneForChildValidator<TChild, TValidator>(TChild child, TValidator childValidator) where TValidator : IDomainObjectFluentValidator<TChild>
        {
            var clone = new DomainValidationContext<TChild>(child, this.PropertyChain, this.Selector, ChildContextType.Child, childValidator, this.RootContext);
            foreach (var data in RootContextData)
                clone.RootContextData.Add(data);
            return clone;
        }

        public DomainValidationContext<TChild> CloneForChildCollectionValidator<TChild, TValidator>(TChild child, TValidator validator) where TValidator : IDomainObjectFluentValidator<TChild>
        {
            var clone = new DomainValidationContext<TChild>(child, this.PropertyChain, this.Selector, ChildContextType.ChildCollection, validator, this.RootContext);
            foreach (var data in RootContextData)
                clone.RootContextData.Add(data);
            return clone;
        }

        public DomainValidationContext<TChild> CloneForPrimitiveValidator<TChild, TValidator>(TChild child) where TValidator : IDomainPrimitiveFluentValidator<TChild>
        {
            return CloneForPrimitiveValidator(child, ResolveChildValidator<TValidator>());
        }

        public DomainValidationContext<TChild> CloneForPrimitiveValidator<TChild, TValidator>(TChild child, TValidator childValidator) where TValidator : IDomainPrimitiveFluentValidator<TChild>
        {
            var clone = new DomainValidationContext<TChild>(child, this.PropertyChain, this.Selector, ChildContextType.Child, childValidator, this.RootContext);
            foreach (var data in RootContextData)
                clone.RootContextData.Add(data);
            return clone;
        }

        public override bool IsChildContext => childContextType == ChildContextType.Child;
        public override bool IsChildCollectionContext => childContextType == ChildContextType.ChildCollection;

        IDomainValidator IDomainValidationContext.Validator => Validator;
    }





    //public interface IRootValidationContext
    //{
    //    IValidatorResolver ValidatorResolver { get; }
    //}

    //public class RootValidationContext<T> : CustomValidationContext<T>, IRootValidationContext
    //{
    //    public RootValidationContext(T instanceToValidate, IDomainObjectFluentValidator<T> validator) 
    //        : base(instanceToValidate, validator)
    //    {
    //    }
    //}
}
