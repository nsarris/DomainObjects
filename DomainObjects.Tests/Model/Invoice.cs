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
        private AggregateList<InvoiceLine> invoiceLines = new AggregateList<InvoiceLine>(); 

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
        public IAggregateReadOnlyList<InvoiceLine> InvoiceLines => invoiceLines;


        public InvoiceLine CreateNewLine()
        {
            var line = linefactory.New.Construct(this);
            this.invoiceLines.Add(line);
            return line;
        }
    }
}
