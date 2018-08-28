using FluentValidation;

namespace DomainObjects.Validation.FluentValidation
{
    public interface IDomainValidationContext : IValidationContext
    {
        IDomainValidator Validator { get; }
        TChildValidator ResolveChildValidator<TChildValidator>() where TChildValidator : IDomainValidator;

        DomainValidationContext<TChild> CloneForChildValidator<TChild, TValidator>(TChild child)
            where TValidator : IDomainFluentValidator<TChild>;
        DomainValidationContext<TChild> CloneForChildValidator<TChild, TValidator>(TChild child, TValidator childValidator)
            where TValidator : IDomainFluentValidator<TChild>;

        DomainValidationContext<TChild> CloneForChildCollectionValidator<TChild, TValidator>(TChild child)
            where TValidator : IDomainFluentValidator<TChild>;
        DomainValidationContext<TChild> CloneForChildCollectionValidator<TChild, TValidator>(TChild child, TValidator validator)
            where TValidator : IDomainFluentValidator<TChild>;

    }
}
