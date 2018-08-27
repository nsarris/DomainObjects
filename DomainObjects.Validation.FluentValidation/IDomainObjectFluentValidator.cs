using DomainObjects.Core;

namespace DomainObjects.Validation.FluentValidation
{
    public interface IDomainObjectFluentValidator<T> : IDomainFluentValidator<T>, IDomainObjectValidator<T>
    {
        DomainValidationResult ValidateChild<TChild, TChildValidator>(TChild childInstance, DomainValidationContext<T> context)
            where TChild : DomainObject
            where TChildValidator : IDomainObjectFluentValidator<TChild>;

        DomainValidationResult ValidatePrimitive<TProperty, TChildValidator>(TProperty value, DomainValidationContext<T> context)
            where TChildValidator : IDomainPrimitiveFluentValidator<TProperty>;
    }
}
