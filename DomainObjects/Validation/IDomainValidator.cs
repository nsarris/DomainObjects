namespace DomainObjects.Validation
{
    public interface IDomainValidator<T>
    {
        DomainValidationResult Validate(T instance);
    }
}
