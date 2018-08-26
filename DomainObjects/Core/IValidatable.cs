using DomainObjects.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Validation
{
    //PropertyValidator (value / collection / entity? / valueType / complex)
    //EntityValidator

    public interface IValidatable
    {
        void Validate();
        //ValidationContext
    }

    public interface IValidationContext<T>
    {
        T Target { get; }
    }

    public interface IValidator<T> where T: DomainObject
    {
        IValidationResult Validate(IValidationContext<T> validationContext);
    }

    public interface IValidationResult
    {
        bool IsValid { get; }
        //List<ValidatorError> Errors
    }

    public class ValidatorResult // ValidatorError
    {
        //Type: Property/Entity
        //Property
        //Message
        //UserMessage
        //Code
        //List<ValidatorResult> Inner/Nested results
    }
}
