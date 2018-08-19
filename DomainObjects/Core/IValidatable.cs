using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Core
{
    //PropertyValidator (value / collection / entity? / valueType / complex)
    //EntityValidator

    public interface IValidatable
    {
        void Validate();
        //ValidationContext
    }

    public class ValidationResult
    {
        public bool Valid { get; private set; }
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
