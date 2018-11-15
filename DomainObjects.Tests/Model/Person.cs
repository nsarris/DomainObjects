using DomainObjects.Core;
using System;

namespace DomainObjects.Tests.Sales
{
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

        protected Person()
        {

        }
        protected Person(int id)
        {
            Id = id;
        }

        //protected Person(SerializationInfo info, StreamingContext context) : base(info, context)
        //{

        //}
    }
}
