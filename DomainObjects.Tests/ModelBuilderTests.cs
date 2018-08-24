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
                //.HasKey(x => new { x.MainAddress })
                .IgnoreMember(x => x.StringComparer)
                ;

            //customerBuilder.HasKey(x => new { x.Id, x.Name });
            //customerBuilder.HasKey(x => new { x.Id });

            var model = modelBuilder.Build();

            var customer = new Customer();

            var metadata = customer.GetEntityMetadata();

            var key1 = customer.GetKey();


            var customer2 = new Customer(0, "No");

            customer2.MainAddress = new Address("1", "1", null, null, null, null);
            customer2.InitNew(false);
            //customer2.SetKey(1, "Nikos");
            customer2.SetKey(1);

            var key2 = customer2.GetKey();

            var customer3 = new Customer(1, "Nikos");

            customer3.MainAddress = new Address("1", "1", null, null, null, null);
            customer3.InitExisting();

            
            //customer2.SetKey("1", "1", null, null, null, null);

            var key3 = customer3.GetKey();

            Assert.AreEqual(key2, key3);

            var keyValue2 = customer2.GetKeyValue();
            var keyValue3 = customer3.GetKeyValue();

            Assert.AreEqual(keyValue2, keyValue3);
        }
    }
}
