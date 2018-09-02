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
    public class Customer : AggregateRoot<Customer,int>
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

    }
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

        public string Street { get; }
        public string Number { get; }
        public string City { get; }
        public string PostCode { get; }
        public Phone PrimaryPhone { get; }
        public ValueObjectReadOnlyList<Phone> OtherPhones { get; }

        
    }
    public class Phone : DomainValueObject<Phone>
    {
        public Phone(string number, int kind)
        {
            Number = number;
            Kind = kind;
        }

        public string Number { get; }
        public int Kind { get; }

    }

    




    [AddINotifyPropertyChangedInterface]
    public class Invoice : AggregateRoot<Invoice, int>
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

        public InvoiceLine CreateNewLine()
        {
            var line = new InvoiceLine(this);
            this.InvoiceLines.Add(line);
            return line;
        }
    }

    [AddINotifyPropertyChangedInterface]
    public class InvoiceLine : Aggregate<InvoiceLine, Invoice, int>
    {
        public int Id { get; private set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public override Invoice Parent { get; }
        public InvoiceLine(Invoice parent)
        {
            this.Parent = parent;
        }

        
    }

    public class Product : AggregateRoot<Product, int>
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
