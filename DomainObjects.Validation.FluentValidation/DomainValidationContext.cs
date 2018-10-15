using FluentValidation;
using FluentValidation.Internal;
using System;
using System.Collections.Generic;

namespace DomainObjects.Validation.FluentValidation
{
    public class DomainValidationContext<T> : ValidationContext<T>, IDomainValidationContext
    {
        public IDomainFluentValidator<T> Validator { get; }
        
        private readonly ChildContextType childContextType = ChildContextType.None;
        public DomainValidationContext(T instanceToValidate, IDomainFluentValidator<T> validator) : base(instanceToValidate)
        {
            Validator = validator;
        }

        public DomainValidationContext(T instanceToValidate, PropertyChain propertyChain, IValidatorSelector validatorSelector, IDomainFluentValidator<T> validator)
            : base(instanceToValidate, propertyChain, validatorSelector)
        {
            Validator = validator;
        }

        public DomainValidationContext(T instanceToValidate, PropertyChain propertyChain, IValidatorSelector validatorSelector, ChildContextType childContextType, IDomainFluentValidator<T> validator)
            : base(instanceToValidate, propertyChain, validatorSelector)
        {
            this.childContextType = childContextType;
            Validator = validator;
        }

        public DomainValidationContext<TChild> CloneForChildValidator<TChild, TValidator>(TChild child)
            where TValidator : IDomainFluentValidator<TChild>
        {
            return CloneForChildCollectionValidator(child, ResolveChildValidator<TValidator>());
        }

        public DomainValidationContext<TChild> CloneForChildCollectionValidator<TChild, TValidator>(TChild child)
            where TValidator : IDomainFluentValidator<TChild>
        {
            return CloneForChildCollectionValidator(child, ResolveChildValidator<TValidator>());
        }

        public TValidator ResolveChildValidator<TValidator>() where TValidator : IDomainValidator
        {
            return Validator.ResolveChildValidator<TValidator>();
        }

        public DomainValidationContext<TChild> CloneForChildValidator<TChild, TValidator>(TChild child, TValidator childValidator) 
            where TValidator : IDomainFluentValidator<TChild>
        {
            var clone = new DomainValidationContext<TChild>(child, this.PropertyChain, this.Selector, ChildContextType.Child, childValidator);
            foreach (var data in RootContextData)
                clone.RootContextData.Add(data);
            return clone;
        }

        public DomainValidationContext<TChild> CloneForChildCollectionValidator<TChild, TValidator>(TChild child, TValidator validator) 
            where TValidator : IDomainFluentValidator<TChild>
        {
            var clone = new DomainValidationContext<TChild>(child, this.PropertyChain, this.Selector, ChildContextType.ChildCollection, validator);
            foreach (var data in RootContextData)
                clone.RootContextData.Add(data);
            return clone;
        }

        public override bool IsChildContext => childContextType == ChildContextType.Child;
        public override bool IsChildCollectionContext => childContextType == ChildContextType.ChildCollection;

        IDomainValidator IDomainValidationContext.Validator => Validator;
    }
}
