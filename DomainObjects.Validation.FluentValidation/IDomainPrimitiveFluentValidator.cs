using FluentValidation.Results;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Validation.FluentValidation
{
    public interface IDomainPrimitiveFluentValidator<T> : IDomainFluentValidator<T>
    {
        
    }
}
