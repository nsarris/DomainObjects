using DomainObjects.Core;
using FluentValidation;

namespace DomainObjects.Validation.FluentValidation
{
    public interface IDomainFluentValidator<T> : IDomainValidator<T>
    {
        AbstractValidator<T> FluentValidator { get; }
        DomainValidationResult Validate(T instance, IDomainValidationContext parentContext);

        DomainValidationResult ValidateChild<TChild, TChildValidator>(TChild childInstance, DomainValidationContext<T> context)
            where TChildValidator : IDomainFluentValidator<TChild>;
    }
}
