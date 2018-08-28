using DomainObjects.Core;
using DomainObjects.Validation.FluentValidation;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using FluentValidation.Validators;
using DomainObjects.Validation;

namespace DomainObjects.Tests.Sales
{
    public class TestService
    {
        public bool IsNegative(int i) => i < 0;
    }

    public class TestService2
    {
        public bool IsPositive(int i) => i > 0;
    }

    public class CityExistanceService
    {
        public bool Exists(string cityName)
        {
            if (string.IsNullOrEmpty(cityName))
                return false;

            return cityName.Length > 3;
        }
    }

    public class CustomerRepository
    {
        public Customer CreateNew()
        {
            var customer = new Customer();
            customer.InitNew(false);
            return customer;
        }

        public Customer GetById(int id)
        {
            var customer = new Customer(id, "Test" + id);
            customer.InitExisting();
            return customer;
        }
    }

    public class InvoiceRepository
    {
        public Invoice CreateNew()
        {
            var invoice = new Invoice();
            invoice.InitNew(false);
            return invoice;
        }

        public Invoice GetById(int id)
        {
            var invoice = new Invoice(id);
            invoice.InitExisting();
            return invoice;
        }
    }

    [AddINotifyPropertyChangedInterface]
    public class Customer : AggregateRoot
    {
        //private string testInnerField  = "test";

        public int Id { get; private set; }
        public string Name { get; set; }
        public Address MainAddress { get; set; }
        public ValueList<Address> OtherAddresses { get; set; }
        public StringComparer StringComparer { get; set; }
        public Customer()
        {

        }
        public Customer(int id, string name)
        {
            Id = id;
            Name = name;
        }



        public class Validator : DomainFluentValidator<Customer>
        {
            public Address.Validator AddressValidator { get; }
            public Validator(TestService someDependency, Address.Validator addressValidator)
                : base(ValidatorImpl.Instance.Value)
            {
                AddressValidator = addressValidator;
                SomeDependency = someDependency;

                AutoRegisterChildValidators();
            }

            public TestService SomeDependency { get; }


            private class ValidatorImpl : AbstractValidator<Customer>
            {
                public static Lazy<ValidatorImpl> Instance = new Lazy<ValidatorImpl>(() => new ValidatorImpl());

                public ValidatorImpl()
                {
                    //TODO: ApplyFrom (EF/Linq2db/Dalia) -> using mapper (get straight properties)


                    //RuleFor(x => x).Custom((x, context) =>
                    //{

                    //});

                    //RuleFor(x => x.Id).GreaterThan(0).Must((customer, x, context) =>
                    //{
                    //    var parentContext = (DomainValidationContext<Customer>)context.ParentContext;
                    ////var validatorResolver = parentContext.ValidatorResolver;
                    ////var validator = validatorResolver.ResolveValidator<Customer.Validator>();
                    //var validator = (Customer.Validator)parentContext.Validator;


                    //    //var dependency = validator.GetDependency<TestService>();
                    //    return !validator.SomeDependency.IsNegative(x);
                    //});

                    //RuleFor(x => x.Name).NotEmpty();

                    RuleFor(x => x.MainAddress).NotNull().AsDomainChild().ValidateUsing<Address.Validator>();
                    //RuleForEach(x => x.OtherAddresses).SetValidator(new Address.Validator());
                }
            }
        }
    }
    public class Address : DomainValue
    {
        public Address(string street, string number, string city, string postCode, Phone primaryPhone, IEnumerable<Phone> otherPhones)
        {
            Street = street;
            Number = number;
            City = city;
            PostCode = postCode;
            PrimaryPhone = primaryPhone;
            OtherPhones = new ValueReadOnlyList<Phone>(otherPhones);
        }

        public string Street { get; }
        public string Number { get; }
        public string City { get; }
        public string PostCode { get; }
        public Phone PrimaryPhone { get; }
        public ValueReadOnlyList<Phone> OtherPhones { get; }

        public class Validator : DomainFluentValidator<Address>
        {
            public Phone.Validator PhoneValidator { get; }
            public CityValidator CityValidator { get; }
            
            public Validator(Phone.Validator phoneValidator, CityValidator cityValidator) : base(ValidatorImpl.Instance.Value)
            {
                PhoneValidator = phoneValidator;
                CityValidator = cityValidator;

                AutoRegisterChildValidators();
            }

            private class ValidatorImpl : AbstractValidator<Address>
            {
                public static Lazy<ValidatorImpl> Instance = new Lazy<ValidatorImpl>(() => new ValidatorImpl());
                public ValidatorImpl()
                {
                    RuleFor(x => x.Street).NotEmpty();
                    RuleFor(x => x.Number).NotEmpty();
                    RuleFor(x => x.City).AsDomainChild().ValidateUsingCustom<CityValidator>();
                    //RuleFor(x => x.PrimaryPhone).SetValidator(new Phone.Validator());
                    //RuleForEach(x => x.OtherPhones).SetValidator(new Phone.Validator());

                    RuleFor(x => x.PrimaryPhone).NotNull().AsDomainChild().ValidateUsing<Phone.Validator>();
                    RuleForEach(x => x.OtherPhones).AsDomainChild().ValidateUsing<Phone.Validator>();
                }
            }
        }
    }
    public class Phone : DomainValue
    {
        public Phone(string number, int kind)
        {
            Number = number;
            Kind = kind;
        }

        public string Number { get; }
        public int Kind { get; }

        public class Validator : DomainFluentValidator<Phone>
        {
            public Validator() : base(ValidatorImpl.Instance.Value)
            {
                AutoRegisterChildValidators();
            }

            private class ValidatorImpl : AbstractValidator<Phone>
            {
                public static Lazy<ValidatorImpl> Instance = new Lazy<ValidatorImpl>(() => new ValidatorImpl());
                public ValidatorImpl()
                {
                    RuleFor(x => x.Number).NotEmpty().Length(5,10);
                }
            }
        }
    }

    public class CityValidator : IDomainValidator<string>
    {
        CityExistanceService cityExistanceService;

        public CityValidator(CityExistanceService cityExistanceService)
        {
            this.cityExistanceService = cityExistanceService;
        }

        public TChildValidator ResolveChildValidator<TChildValidator>() where TChildValidator : IDomainValidator
        {
            throw new NotSupportedException("CityValidator does not have any child validators");
        }

        public DomainValidationResult Validate(string instance)
        {
            var r = new DomainValidationResult();
            if (!cityExistanceService.Exists(instance))
                r.AddFailure(new DomainValidationError("City",$"City '{instance}' does not exist", instance));
            return r;
        }
    }




    [AddINotifyPropertyChangedInterface]
    public class Invoice : AggregateRoot
    {
        public Invoice()
        {

        }

        public Invoice(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }
        public int CustomerId { get; set; }
        public DateTime DateTime { get; set; }
        public AggregateList<InvoiceLine> InvoiceLines { get; } = new AggregateList<InvoiceLine>();
    }

    [AddINotifyPropertyChangedInterface]
    public class InvoiceLine : Aggregate
    {
        public int Id { get; private set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
    }

    public class Product : AggregateRoot
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
