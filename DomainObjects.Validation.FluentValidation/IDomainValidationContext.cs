using FluentValidation;

namespace DomainObjects.Validation.FluentValidation
{
    public interface IDomainValidationContext : IValidationContext
    {
        IDomainValidationContext RootContext { get; }
        new IDomainValidationContext ParentContext { get; }
        IDomainValidator Validator { get; }
        TChildValidator ResolveChildValidator<TChildValidator>() where TChildValidator : IDomainValidator;
        DomainValidationContext<TChild> CloneForChildValidator<TChild, TValidator>(TChild child)
            where TValidator : IDomainObjectFluentValidator<TChild>;
        DomainValidationContext<TChild> CloneForChildValidator<TChild, TValidator>(TChild child, TValidator childValidator)
            where TValidator : IDomainObjectFluentValidator<TChild>;

        DomainValidationContext<TChild> CloneForChildCollectionValidator<TChild, TValidator>(TChild child)
            where TValidator : IDomainObjectFluentValidator<TChild>;
        DomainValidationContext<TChild> CloneForChildCollectionValidator<TChild, TValidator>(TChild child, TValidator validator)
            where TValidator : IDomainObjectFluentValidator<TChild>;

        DomainValidationContext<TChild> CloneForPrimitiveValidator<TChild, TValidator>(TChild child)
            where TValidator : IDomainPrimitiveFluentValidator<TChild>;
        DomainValidationContext<TChild> CloneForPrimitiveValidator<TChild, TValidator>(TChild child, TValidator childValidator)
            where TValidator : IDomainPrimitiveFluentValidator<TChild>;
    }
}
