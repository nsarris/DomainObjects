using DomainObjects.Core;
using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainObjects.Validation
{
    public abstract class DomainValidatorBase<T> : IDomainValidator<T>
    {
        private readonly Dictionary<Type, object> childValidators = new Dictionary<Type, object>();

        protected DomainValidatorBase()
        {
            
        }

        protected void AutoRegisterChildValidators()
        {
            foreach (var prop in
                this.GetType().GetPropertiesEx()
                .Where(x => x.Type.IsAssignableTo(typeof(IDomainValidator))))
            {
                //check type compatibility 
                RegisterChildValidator(prop.Get<IDomainValidator>(this));
            }
        }

        public TValidator ResolveChildValidator<TValidator>() where TValidator : IDomainValidator
        {
            if (childValidators.TryGetValue(typeof(TValidator), out var childValidator))
                return (TValidator)childValidator;

            throw new KeyNotFoundException($"No child validator of type '{typeof(TValidator).FullName}' registered in validator '{this.GetType().FullName}'. Either call AutoRegisterChildContainers() at the end of construction or register child containers manually.");
        }

        private void RegisterChildValidator(IDomainValidator validator)
        {
            //check type compatibility 
            //assert not null
            childValidators[validator.GetType()] = validator;
        }

        public void RegisterChildValidator<TChild>(IDomainValidator<TChild> validator) where TChild : DomainObject
        {
            //check type compatibility
            //assert not null
            childValidators[validator.GetType()] = validator;
        }

        public abstract DomainValidationResult Validate(T instance);
    }
}
