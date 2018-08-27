using FluentValidation;

namespace DomainObjects.Validation.FluentValidation
{
    public class DomainPrimitiveFluentValidator<T> : DomainPrimitiveValidatorBase<T>, IDomainPrimitiveFluentValidator<T> where T : DomainObjects.Core.DomainObject
    {
        public AbstractValidator<T> FluentValidator { get; }

        public DomainPrimitiveFluentValidator(AbstractValidator<T> fluentValidator)
        {
            FluentValidator = fluentValidator;
        }

        public override DomainValidationResult Validate(T instance)
        {
            var context = new DomainValidationContext<T>(instance, this);
            return FluentValidator.Validate(context).ToDomainValidationResult();
        }

        public DomainValidationResult Validate(T instance, IDomainValidationContext parentContext)
        {
            var clonedChildContext = parentContext.CloneForPrimitiveValidator(instance, this);
            return FluentValidator.Validate(clonedChildContext).ToDomainValidationResult();
        }
    }
}
