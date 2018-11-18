using DomainObjects.Core;

namespace DomainObjects.Tests.Sales
{
    //[AddINotifyPropertyChangedInterface]
    //[Serializable]
    public class InvoiceLine : Aggregate<InvoiceLine, Invoice, int>
    {
        public virtual int Id { get; private set; }
        public virtual int ProductId { get; set; }
        public virtual decimal Quantity { get; set; }
        
        protected InvoiceLine(Invoice parent)
            :base(parent)
        {
            
        }

        //protected InvoiceLine(SerializationInfo info, StreamingContext context) : base(info, context)
        //{

        //}
    }
}
