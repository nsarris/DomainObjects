using DomainObjects.Internal;
using DomainObjects.ModelBuilder;
using DomainObjects.Serialization;
using DomainObjects.Tests.Sales;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Tests
{
    [TestFixture]
    class SerializationTests
    {
        [OneTimeSetUp]
        public void Setup()
        {
            var modelBuilder = new DomainModelBuilder()
                .HasModelName("Sales");

            var invoiceBuilder = modelBuilder.Entity<Invoice>().HasKey(x => x.Id);
            var invoiceLineBuilder = modelBuilder.Entity<InvoiceLine>().HasKey(x => x.Id);
            var productBuilder = modelBuilder.Entity<Product>().HasKey(x => x.Id);

            var customerBuilder = modelBuilder.Entity<Customer>().HasKey(x => x.Id)
                .IgnoreMember(x => x.StringComparer)
                ;

            var model = modelBuilder.Build();
        }

        [Test]
        public void SerializationTest()
        {
            var customer2 = new Customer(0, "No");

            customer2.MainAddress = new Address("1", "1", null, null, new Phone("!23",1), null);
            customer2.OtherAddresses.AddRange(new [] {
                new Address("2", "2", null, null, new Phone("9123", 1), null),
                new Address("3", "3", null, null, new Phone("123123", 1), null),
                });
            var ser = JsonConvert.SerializeObject(customer2);

            var customer2Copy = JsonConvert.DeserializeObject<Customer>(ser);
            Assert.IsTrue(ObjectComparer.Default.DeepEquals(customer2, customer2Copy));
        }

        [Test]
        public void DomainSerializerTest()
        {
            var serializer = new DomainSerializer();
            var customerRepository = new CustomerRepository();


            var customer2 = customerRepository.CreateNew(0, "No");

            customer2.MainAddress = new Address("1", "1", null, null, new Phone("!23", 1), null);
            customer2.OtherAddresses.AddRange(new[] {
                new Address("2", "2", null, null, new Phone("9123", 1), null),
                new Address("3", "3", null, null, new Phone("123123", 1), null),
                });
            var ser = serializer.Serialize(customer2);

            var customer2Copy = serializer.Deserialize<Customer>(ser);
            Assert.IsTrue(ObjectComparer.Default.DeepEquals(customer2, customer2Copy));
        }
    }
}
