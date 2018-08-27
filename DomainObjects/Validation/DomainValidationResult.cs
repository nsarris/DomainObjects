using System.Collections.Generic;

namespace DomainObjects.Validation
{
    public class DomainValidationResult
    {
        readonly List<DomainValidationError> errors = new List<DomainValidationError>();
        public bool IsValid { get; private set; } = true;

        public IEnumerable<DomainValidationError> Errors => errors;

        public DomainValidationResult()
        {

        }

        public void AddFailure(DomainValidationError error)
        {
            errors.Add(error);
            IsValid = false;
        }

        public void AddNonFailure(DomainValidationError error)
        {
            errors.Add(error);
        }
    }
}
