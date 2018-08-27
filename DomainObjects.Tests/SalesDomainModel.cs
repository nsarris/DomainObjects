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

namespace DomainObjects.Tests.Sales
{

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

        public class TestService
        {
            public bool IsNegative(int i) => i < 0;
        }

        public class Validator : DomainValidator<Customer>
        {
            public Validator(TestService someDependency)
                :base(FluentValidator.Instance.Value)
            {
                SomeDependency = someDependency;

                RegisterDependency(SomeDependency ?? new TestService());
            }

            public new FluentValidator FluentValidator => (FluentValidator)base.FluentValidator;
            public TestService SomeDependency { get; }
        }
        
        public class FluentValidator : AbstractValidator<Customer>
        {
            public static Lazy<FluentValidator> Instance = new Lazy<FluentValidator>(() => new FluentValidator());

            //protected override bool PreValidate(ValidationContext<Customer> context, ValidationResult result)
            //{
            //    var parentContext = (CustomValidationContext<Customer>)context;
            //    var validatorResolver = parentContext.ValidatorResolver;
            //    var validator = validatorResolver.ResolveValidator<Customer.Validator>();
            //    //Get and inject dependencies
            //    parentContext.RootContextData.Add("Test", validator.SomeDependency);

            //    return base.PreValidate(context, result);
            //}

            public FluentValidator()
            {
                

                RuleFor(x => x).Custom((x, context) =>
                {
                    //x.Validate() //From Domain
                    
                    
                    
                    //var parentContext = (CustomValidationContext<Customer>)context.ParentContext;
                    //var validatorResolver = parentContext.ValidatorResolver;
                    //var validator = validatorResolver.ResolveValidator<Customer.Validator>();
                    ////Get and inject dependencies
                    //parentContext.RootContextData.Add("Test", validator.SomeDependency);
                    
                    //var result = addressValidator.Validate(clonedChildContext);
                    //foreach (var failure in result.Errors)
                    //    context.AddFailure(failure);
                });

                RuleFor(x => x.Id).GreaterThan(0).Must((customer, x, context) =>
                {
                    var parentContext = (CustomValidationContext<Customer>)context.ParentContext;
                    var validatorResolver = parentContext.ValidatorResolver;
                    var validator = validatorResolver.ResolveValidator<Customer.Validator>();

                    var dependency = validator.GetDependency<TestService>();
                    return !dependency.IsNegative(x);
                });

                RuleFor(x => x.Name).NotEmpty();
                //RuleFor(x => x.testInnerField).NotEmpty();
                //RuleFor(x => x.MainAddress).SetValidator(new Address.Validator());

                RuleFor(x => x.MainAddress).Custom((x, context) =>
                {
                    var parentContext = (CustomValidationContext<Customer>)context.ParentContext;
                    //parentContext.RootContextData.Add("dsd", 0);
                    //var clonedChildContext1 = parentContext.CloneForChildValidator(x, true, context.ParentContext.Selector);
                    var clonedChildContext = parentContext.CloneForChildValidator<Address>(x);
                    //var childContext = new CustomValidationContext<Address>(x, parentContext.PropertyChain, parentContext.Selector,);
                    

                    var validatorResolver = parentContext.ValidatorResolver;
                    //var addressValidator = validatorResolver.ResolveValidator<Address.Validator>();

                    //var result = addressValidator.Validate(clonedChildContext);
                    //foreach (var failure in result.Errors)
                    //    context.AddFailure(failure);
                });

                RuleForEach(x => x.OtherAddresses).SetValidator(new Address.Validator());
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

        public class Validator : DomainObjectFluentValidator<Phone>
        {
            public Validator()
            {
                RuleFor(x => x.Number).NotEmpty();
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

        public class Validator : AbstractValidator<Address>
        {
            public Validator()
            {
                RuleFor(x => x.Street).NotEmpty();
                RuleFor(x => x.Number).NotEmpty();
                RuleFor(x => x.City).NotEmpty();
                RuleFor(x => x.PrimaryPhone).SetValidator(new Phone.Validator());
                RuleForEach(x => x.OtherPhones).SetValidator(new Phone.Validator());
            }
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
