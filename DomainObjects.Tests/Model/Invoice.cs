using DomainObjects.Core;
using System;

namespace DomainObjects.Tests.Sales
{
    //[AddINotifyPropertyChangedInterface]
    //[Serializable]
    public class Invoice : AggregateRoot<Invoice, int>
    {
        DomainEntityFactory<InvoiceLine, Invoice> linefactory = new DomainEntityFactory<InvoiceLine, Invoice>();

        private int entityState = 3;
        protected Invoice()
        {

        }

        protected Invoice(int id)
        {
            Id = id;
        }

        //protected Invoice(SerializationInfo info, StreamingContext context) : base(info, context)
        //{

        //}

        public virtual int Id { get; private set; }
        public virtual int CustomerId { get; set; }
        public virtual DateTime DateTime { get; set; }
        public AggregateList<InvoiceLine> InvoiceLines { get; } = new AggregateList<InvoiceLine>();


        public InvoiceLine CreateNewLine()
        {
            var line = linefactory.New.Construct(this);
            this.InvoiceLines.Add(line);
            return line;
        }
    }
}
