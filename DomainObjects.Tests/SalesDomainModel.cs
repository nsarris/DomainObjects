﻿using DomainObjects.Core;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Tests.Sales
{

    [AddINotifyPropertyChangedInterface]
    public class Customer : AggregateRoot
    {
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

    public class Phone : DomainValue
    {
        public Phone(string number, int kind)
        {
            Number = number;
            Kind = kind;
        }

        public string Number { get; }
        public int Kind { get; }
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
    }

    [AddINotifyPropertyChangedInterface]
    public class Invoice : AggregateRoot
    {
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
