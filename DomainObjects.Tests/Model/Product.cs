using DomainObjects.Core;

namespace DomainObjects.Tests.Sales
{
    public class Product : AggregateRoot<Product, int>
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        //public Product()
        //{

        //}

        //protected Product(SerializationInfo info, StreamingContext context) : base(info, context)
        //{

        //}
    }
}
