using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainObjects.Core;
using DomainObjects.Validation;
using FluentValidation;

namespace DomainObjects.Validation.FluentValidation
{
    public class DomainObjectFluentValidator<T> : AbstractValidator<T>, IValidator<T> where T : DomainObject
    {
        IValidationResult IValidator<T>.Validate(IValidationContext<T> validationContext)
        {
            //var context = new DomainValidationContext<T>()
            var context = (DomainValidationContext<T>)validationContext;

            var result = this.Validate(context);

            return null;
        }
    }
}
