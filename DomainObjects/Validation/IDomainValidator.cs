namespace DomainObjects.Validation
{
    public interface IDomainValidator
    {
        
    }

    public interface IDomainValidator<T> : IDomainValidator
    {
        DomainValidationResult Validate(T instance);
    }

    public interface IDomainObjectValidator : IDomainValidator
    {
        TChildValidator ResolveChildValidator<TChildValidator>() where TChildValidator : IDomainValidator;
    }

    public interface IDomainObjectValidator<T> : IDomainObjectValidator, IDomainValidator<T>
    {
        
    }

    public interface IDomainPrimitiveValidator : IDomainValidator
    {
        
    }

    public interface IDomainPrimitiveValidator<T> : IDomainPrimitiveValidator,IDomainValidator<T>
    {
        
    }
}
