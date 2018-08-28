using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.IoC;
using FluentValidation.Results;
using FluentValidation.Validators;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Tests.FluentValidationDI
{

    public class CustomerValidator : AbstractValidator<Sales.Customer>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.MainAddress)
                .NotNull()
                .WithIoC()
                    .CustomUsing<AddressValidatorDependency>((parent, x, dependency) =>
                    {
                        return true;
                    })
                    .SetValidator<AddressValidator>()
                    .Custom((parent, x, resolver) =>
                    {
                        return true;
                    });
        }
    }


    public class AddressValidatorDependency
    {
        public Sales.TestService SomeDependency { get; }
        public AddressValidatorDependency(Sales.TestService testService)
        {
            SomeDependency = testService;
        }
    }

    public class AddressValidator : AbstractValidator<Sales.Address>
    {
        public AddressValidator()
        {
            RuleFor(x => x.City)
                .WithIoC()
                    .Custom((parent, x, resolver) =>
                    {
                        return true;
                    });

            RuleFor(x => x.PrimaryPhone)
                .WithIoC()
                    .SetValidator<PhoneValidator>()
                    .Custom((parent, x, resolver) =>
                    {
                        return true;
                    });

            RuleForEach(x => x.OtherPhones)
                .WithIoC()
                    .SetValidator<PhoneValidator>();
        }
    }

    public class PhoneValidator : AbstractValidator<Sales.Phone>
    {
        public PhoneValidator()
        {
            RuleFor(x => x.Number)
                .NotEmpty()
                .Length(5, 10)
                .WithIoC()
                    .Custom((parent, x, resolver) =>
                    {
                        return x.Length > 2;
                    });
        }
    }
}
