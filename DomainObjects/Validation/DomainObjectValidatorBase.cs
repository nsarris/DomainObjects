using DomainObjects.Core;

namespace DomainObjects.Validation
{
    public abstract class DomainObjectValidatorBase<T> : DomainValidatorBase<T>, IDomainObjectValidator<T>
        where T : DomainObject
    {
    }
}
