using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using DomainObjects;
using DomainObjects.Core;
using DomainObjects.ModelBuilder;
using DomainObjects.Internal;
using DomainObjects.Tests.Sales;

namespace DomainObjects.Tests
{
    [TestFixture]
    public class ModelBuilderTests
    {
        [Test]
        public void TestBuilder()
        {
            var modelBuilder = new DomainModelBuilder()
                .HasModelName("Sales");

            var invoiceBuilder = modelBuilder.Entity<Invoice>().HasKey(x => x.Id);
            var invoiceLineBuilder = modelBuilder.Entity<InvoiceLine>().HasKey(x => x.Id);
            var productBuilder = modelBuilder.Entity<Product>().HasKey(x => x.Id);

            var customerBuilder = modelBuilder
                .Entity<Customer>()
                .HasKey(x => new { x.Id })
                .IgnoreMember(x => x.StringComparer)
                ;

            //customerBuilder.HasKey(x => new { x.Id, x.Name });
            //customerBuilder.HasKey(x => new { x.Id });

            var model = modelBuilder.Build();

            var customer = new Customer();

            var metadata = customer.GetEntityMetadata();

            var key1 = customer.GetKey();

            //TODO: Key
            //customer.SetKey(1, "Nikos");

            var customer2 = new Customer(1, "Nikos");

            var key2 = customer2.GetKey();
        }
    }
}
