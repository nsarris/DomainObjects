using DomainObjects.ModelBuilder;
using DomainObjects.Tests.Sales;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Tests
{
    [TestFixture]
    public class EntityStateTests
    {
        [Test]
        public void TestEntityStates()
        {
            var repo = new CustomerRepository();
            var customer = repo.CreateNew();

            Assert.IsTrue(customer.GetObjectState() == Core.DomainObjectState.New);

            customer.MarkExisting();

            Assert.IsTrue(customer.GetObjectState() == Core.DomainObjectState.Existing);

            customer.MarkDeleted();

            Assert.IsTrue(customer.GetObjectState() == Core.DomainObjectState.Deleted);
        }

        [Test]
        public void TestPropertyChange()
        {
            var repo = new CustomerRepository();
            var customer = repo.CreateNew();

            Assert.IsTrue(customer.GetObjectState() == Core.DomainObjectState.New);
            Assert.IsFalse(customer.GetIsChanged());

            customer.Name = "New Name";

            Assert.IsTrue(customer.GetIsChanged());

            customer.MainAddress = new Address("", "", "", "", null, null);

            customer.OnPersisted();

            Assert.IsTrue(customer.GetObjectState() == Core.DomainObjectState.Existing);
            Assert.IsFalse(customer.GetIsChanged());
        }

        [Test]
        public void TestAggregateChange()
        {
            var repo = new InvoiceRepository();
            var invoice = repo.CreateNew();

            Assert.IsTrue(invoice.GetObjectState() == Core.DomainObjectState.New);
            Assert.IsFalse(invoice.GetIsChanged());
            Assert.IsFalse(invoice.GetIsChangedDeep());

            var line = invoice.CreateNewLine();
            line.Quantity = 1;
            line.ProductId = 1;

            invoice.OnPersisted();

            Assert.IsTrue(invoice.GetObjectState() == Core.DomainObjectState.Existing);
            Assert.IsFalse(invoice.GetIsChangedDeep());
        }
    }
}
 