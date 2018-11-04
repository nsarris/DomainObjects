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
    public class TrackableTests
    {
        [Test]
        public void TestNewEntity()
        {
            var repo = new CustomerRepository();
            var customer = repo.CreateNew(); 

            Assert.IsTrue(customer.GetEntityState() == Core.EntityState.New);
            Assert.IsFalse(customer.ChangeTracker.GetIsChanged());
            
            customer.Name = "New Name";

            Assert.IsTrue(customer.ChangeTracker.GetIsChanged());
        }

        [Test]
        public void TestPropertyChange()
        {
            var repo = new CustomerRepository();
            var customer = repo.GetById(1);

            Assert.IsTrue(customer.GetEntityState() == Core.EntityState.Existing);
            Assert.IsFalse(customer.ChangeTracker.GetIsChanged());

            customer.Name = "New Name";

            Assert.IsTrue(customer.ChangeTracker.GetIsChanged());

            customer.MainAddress = new Address("", "", "", "", null, null);
            customer.ChangeTracker.AcceptChangesDeep();

            Assert.IsFalse(customer.ChangeTracker.GetIsChanged());

            customer.MainAddress = new Address("", "", "", "", null, null);

            //var a1 = new Address("", "", "", "", null, null);
            //var a2 = new Address("", "", "", "", null, null);

            //Assert.IsTrue(a1 == a2);
            //Assert.IsTrue(a1.Equals(a2));
            //Assert.IsTrue(object.Equals(a1, a2));
            //Assert.IsTrue(EqualityComparer<Address>.Default.Equals(a1, a2));
            //Assert.IsFalse(object.ReferenceEquals(a1, a2));

            Assert.IsFalse(customer.ChangeTracker.GetIsChanged());
        }

        [Test]
        public void TestAggregateChange()
        {
            

            var repo = new InvoiceRepository();
            var invoice = repo.GetById(1);

            Assert.IsTrue(invoice.GetEntityState() == Core.EntityState.Existing);
            Assert.IsFalse(invoice.ChangeTracker.GetIsChanged());
            Assert.IsFalse(invoice.ChangeTracker.GetIsChangedDeep());

            var line = invoice.CreateNewLine();
            line.Quantity = 1;
            line.ProductId = 1;
            
            Assert.IsTrue(invoice.ChangeTracker.GetIsChangedDeep());

            invoice.ChangeTracker.AcceptChangesDeep();
            Assert.IsFalse(invoice.ChangeTracker.GetIsChangedDeep());

            invoice.InvoiceLines.First().Quantity = 3;
            Assert.IsTrue(invoice.ChangeTracker.GetIsChangedDeep());

            invoice.ChangeTracker.AcceptChangesDeep();
            Assert.IsFalse(invoice.ChangeTracker.GetIsChangedDeep());
        }
    }
}
 