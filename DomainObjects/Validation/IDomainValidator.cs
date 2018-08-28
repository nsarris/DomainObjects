namespace DomainObjects.Validation
{
    public interface IDomainValidator
    {
        TChildValidator ResolveChildValidator<TChildValidator>() where TChildValidator : IDomainValidator;
    }

    public interface IDomainValidator<T> : IDomainValidator
    {
        DomainValidationResult Validate(T instance);
    }
}
