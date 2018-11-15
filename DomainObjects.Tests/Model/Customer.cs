using DomainObjects.Core;
using System;

namespace DomainObjects.Tests.Sales
{
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

        //protected Customer(SerializationInfo info, StreamingContext context) : base(info, context)
        //{

        //}
    }
}
