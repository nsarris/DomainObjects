using DomainObjects.Core;
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
            var customer = repo.Create();

            Assert.IsTrue(customer.GetEntityState() == EntityState.New);

            customer.MarkExisting();

            Assert.IsTrue(customer.GetEntityState() == EntityState.Existing);

            customer.MarkDeleted();

            Assert.IsTrue(customer.GetEntityState() == EntityState.Deleted);
        }

        [Test]
        public void TestPropertyChange()
        {
            var repo = new CustomerRepository();
            var customer = repo.Create();

            Assert.IsTrue(customer.GetEntityState() == EntityState.New);
            Assert.IsFalse(customer.ChangeTracker.GetIsChanged());

            customer.Name = "New Name";

            Assert.IsTrue(customer.ChangeTracker.GetIsChanged());

            customer.MainAddress = new Address("", "", "", "", null, null);

            customer.OnPersisted();

            Assert.IsTrue(customer.GetEntityState() == EntityState.Existing);
            Assert.IsFalse(customer.ChangeTracker.GetIsChanged());
        }

        [Test]
        public void TestAggregateChange()
        {
            var repo = new InvoiceRepository();
            var invoice = repo.Create();

            Assert.IsTrue(invoice.GetEntityState() == EntityState.New);
            Assert.IsFalse(invoice.ChangeTracker.GetIsChanged());
            Assert.IsFalse(invoice.ChangeTracker.GetIsChangedDeep());

            var line = invoice.CreateNewLine();
            line.Quantity = 1;
            line.ProductId = 1;

            invoice.OnPersisted();

            Assert.IsTrue(invoice.GetEntityState() == EntityState.Existing);
            Assert.IsFalse(invoice.ChangeTracker.GetIsChangedDeep());
        }
    }
}
 