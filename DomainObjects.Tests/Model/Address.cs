using DomainObjects.Core;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DomainObjects.Tests.Sales
{
    //[Serializable]
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

        //protected Address(SerializationInfo info, StreamingContext context) : base(info, context)
        //{

        //}

        public string Street { get; }
        public string Number { get; }
        public string City { get; }
        public string PostCode { get; }
        public Phone PrimaryPhone { get; }
        public ValueObjectReadOnlyList<Phone> OtherPhones { get; } = new ValueObjectReadOnlyList<Phone>();
    }
}
