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


            var customer = customerRepository.CreateNew(0, "No");

            customer.MainAddress = new Address("1", "1", null, null, new Phone("!23", 1), null);
            customer.OtherAddresses.AddRange(new[] {
                new Address("2", "2", null, null, new Phone("9123", 1), null),
                new Address("3", "3", null, null, new Phone("123123", 1), null),
                });
            var serializedCustomer = serializer.Serialize(customer);

            var customerCopy = serializer.Deserialize<Customer>(serializedCustomer);
            Assert.IsTrue(ObjectComparer.Default.DeepEquals(customer, customerCopy));

            var invoiceRepository = new InvoiceRepository();

            var invoice = invoiceRepository.CreateNew();
            var line = invoice.CreateNewLine();
            line.Quantity = 0.5m;
            line.ProductId = 1;
            line.SetKey(1);

            line = invoice.CreateNewLine();
            line.Quantity = 0.7m;
            line.ProductId = 2;
            line.SetKey(2);
            
            var serializedInvoice = serializer.Serialize(invoice);

            var invoiceCopy = serializer.Deserialize<Invoice>(serializedInvoice);
            Assert.IsTrue(ObjectComparer.Default.DeepEquals(invoice, invoiceCopy));
        }
    }
}
