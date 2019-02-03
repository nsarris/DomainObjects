using DomainObjects.Core;
using System;
using System.Runtime.Serialization;

namespace DomainObjects.Tests.Sales
{
    //[Serializable]
    public class Phone : DomainValueObject<Phone>
    {
        public Phone(string number, int kind)
        {
            Number = number;
            Kind = kind;
        }

        //protected Phone(SerializationInfo info, StreamingContext context) : base(info, context)
        //{

        //}

        public string Number { get; }
        public int Kind { get; }

    }
}
