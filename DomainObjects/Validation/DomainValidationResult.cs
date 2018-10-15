using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainObjects.Validation
{
    public class DomainValidationResult
    {
        public static DomainValidationResult Success { get; } = new DomainValidationResult();

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

        public string ToString(string separator, bool forUser = false)
        {
            return IsValid ? "Valid" : string.Join(separator, errors.Select(x => forUser ? x.UserMessage : x.Message));
        }

        public override string ToString()
        {
            return ToString(Environment.NewLine, false);
        }

        public string ToStringForUser(string separator)
        {
            return ToString(separator, true);
        }

        public string ToStringForUser()
        {
            return ToString(Environment.NewLine, true);
        }
    }
}
