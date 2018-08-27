using FluentValidation;

namespace DomainObjects.Validation.FluentValidation
{
    public interface IDomainFluentValidator<T> : IDomainValidator<T>
    {
        AbstractValidator<T> FluentValidator { get; }
        DomainValidationResult Validate(T instance, IDomainValidationContext parentContext);
    }
}
