using DomainObjects.Core;
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
using System.Runtime.Serialization;

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

    public class CityExistenceService
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
        DomainEntityFactory<Customer, int, string> factory = new DomainEntityFactory<Customer, int, string>();
        public Customer CreateNew()
        {
            return factory.New.Construct(0, null);
        }

        public Customer CreateNew(int id, string name)
        {
            return factory.New.Construct(id, name);
        }

        public Customer GetById(int id)
        {
            return factory.Existing.ConstructWithDefaults(("id", id));
        }
    }

    public class InvoiceRepository
    {
        DomainEntityFactory<Invoice, int> factory = new DomainEntityFactory<Invoice, int>();
        public Invoice CreateNew()
        {
            return factory.New.Construct(0);
        }

        public Invoice GetById(int id)
        {
            return factory.Existing.Construct(id);
        }
    }

    //[AddINotifyPropertyChangedInterface]
    //[Serializable]
    public class Person : AggregateRoot<Customer, int>
    {
        public virtual int Id { get; protected set; }
        public virtual string Surname { get; set; }
        public virtual string Firstname { get; set; }

        private string privateField = nameof(privateField);
        private string privateReadOnlyField = nameof(privateReadOnlyField);
        private string PrivateProperty { get; set; } = nameof(PrivateProperty);
        private string PrivateReadOnlyProperty { get; } = nameof(PrivateReadOnlyProperty);
        private string PrivateReturnOnlyProperty => DateTime.Now.ToString();

        public Person()
        {

        }
        public Person(int id)
        {
            Id = id;
        }

        protected Person(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }

    public class Vendor : Person
    {

    }

    public class Customer : Person
    {
        //private string privateField = nameof(privateField) + "child";

        public virtual int Code { get; set; }
        public virtual decimal Quantity { get; set; }
        public virtual string Name { get; set; }
        public virtual Address MainAddress { get; set; }
        //public ValueList<Address> OtherAddresses { get; set; } = new ValueList<Address>();
        //[DeserializeAs(typeof(ValueList<Address>))]
        public ValueObjectList<Address> OtherAddresses { get; set; } = new ValueObjectList<Address>();
        public StringComparer StringComparer { get; set; }
        public Customer()
        {

        }

        public Customer(int id, string name) : base(id)
        {
            Name = name;
        }

        protected Customer(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }

    [Serializable]
    public class Address : DomainValueObject<Address>
    {
        public Address(string street, string number, string city, string postCode, Phone primaryPhone, IEnumerable<Phone> otherPhones)
        {
            Street = street;
            Number = number;
            City = city;
            PostCode = postCode;
            PrimaryPhone = primaryPhone;
            OtherPhones = new ValueObjectReadOnlyList<Phone>(otherPhones);
        }

        protected Address(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public string Street { get; }
        public string Number { get; }
        public string City { get; }
        public string PostCode { get; }
        public Phone PrimaryPhone { get; }
        public ValueObjectReadOnlyList<Phone> OtherPhones { get; } = new ValueObjectReadOnlyList<Phone>();


    }
    [Serializable]
    public class Phone : DomainValueObject<Phone>
    {
        public Phone(string number, int kind)
        {
            Number = number;
            Kind = kind;
        }

        protected Phone(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public string Number { get; }
        public int Kind { get; }

    }






    //[AddINotifyPropertyChangedInterface]
    [Serializable]
    public class Invoice : AggregateRoot<Invoice, int>
    {
        public Invoice()
        {

        }

        public Invoice(int id)
        {
            Id = id;
        }

        protected Invoice(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public virtual int Id { get; private set; }
        public virtual int CustomerId { get; set; }
        public virtual DateTime DateTime { get; set; }
        public AggregateList<InvoiceLine> InvoiceLines { get; } = new AggregateList<InvoiceLine>();

        [NonSerialized]
        DomainEntityFactory<InvoiceLine, Invoice> linefactory = new DomainEntityFactory<InvoiceLine, Invoice>();

        public InvoiceLine CreateNewLine()
        {
            var line = linefactory.New.Construct(this);
            this.InvoiceLines.Add(line);
            return line;
        }
    }

    //[AddINotifyPropertyChangedInterface]
    [Serializable]
    public class InvoiceLine : Aggregate<InvoiceLine, Invoice, int>
    {
        public virtual int Id { get; private set; }
        public virtual int ProductId { get; set; }
        public virtual decimal Quantity { get; set; }
        
        public InvoiceLine(Invoice parent)
            :base(parent)
        {
            
        }

        protected InvoiceLine(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }

    public class Product : AggregateRoot<Product, int>
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public Product()
        {

        }

        protected Product(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}
