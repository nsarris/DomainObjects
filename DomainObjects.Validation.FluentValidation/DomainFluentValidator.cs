using DomainObjects.Core;
using FluentValidation;

namespace DomainObjects.Validation.FluentValidation
{
    public class DomainFluentValidator<T> : DomainValidatorBase<T>, IDomainFluentValidator<T>
    {
        public DomainFluentValidator(AbstractValidator<T> fluentValidator)
        {
            FluentValidator = fluentValidator;
        }

        public AbstractValidator<T> FluentValidator { get; }

        public override DomainValidationResult Validate(T instance)
        {
            var context = new DomainValidationContext<T>(instance, this);

            //TODO: Use attribute to insert in root context data with name
            //Note: FluentValidation specific
            //foreach (var item in Dependencies)
            //  context.RootContextData.Add(item.Key.Name, item.Value);
            return FluentValidator.Validate(context).ToDomainValidationResult();
        }

        public DomainValidationResult Validate(T instance, IDomainValidationContext parentContext)
        {
            var clonedChildContext = parentContext.CloneForChildValidator(instance, this);
            return FluentValidator.Validate(clonedChildContext).ToDomainValidationResult();
        }

        public DomainValidationResult ValidateChild<TChild, TChildValidator>(TChild childInstance, DomainValidationContext<T> context)
            where TChildValidator : IDomainFluentValidator<TChild>
        {
            var childValidator = context.ResolveChildValidator<TChildValidator>();
            return childValidator.Validate(childInstance, context);
        }
    }
}
