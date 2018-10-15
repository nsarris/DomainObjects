namespace DomainObjects.Validation
{
    public interface IDomainValidator<in T>
    {
        DomainValidationResult Validate(T instance);
    }
}
