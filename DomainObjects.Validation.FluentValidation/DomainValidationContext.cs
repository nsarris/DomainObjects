using FluentValidation;
using FluentValidation.Internal;

namespace DomainObjects.Validation.FluentValidation
{
    public class DomainValidationContext<T> : ValidationContext<T>, IValidationContext<T>
    {
        public DomainValidationContext(T instanceToValidate) : base(instanceToValidate)
        {
        }

        public DomainValidationContext(T instanceToValidate, PropertyChain propertyChain, IValidatorSelector validatorSelector) : base(instanceToValidate, propertyChain, validatorSelector)
        {
            
        }

        T IValidationContext<T>.Target => this.InstanceToValidate;
    }
}
