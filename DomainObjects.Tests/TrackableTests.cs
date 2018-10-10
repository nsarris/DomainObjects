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
        public void TestNewEntity()
        {
            var repo = new CustomerRepository();
            var customer = repo.CreateNew();

            Assert.IsTrue(customer.GetObjectState() == Core.DomainObjectState.New);
            Assert.IsFalse(customer.GetIsChanged());
            
            customer.Name = "New Name";

            Assert.IsTrue(customer.GetIsChanged());
        }

        [Test]
        public void TestPropertyChange()
        {
            var repo = new CustomerRepository();
            var customer = repo.GetById(1);

            Assert.IsTrue(customer.GetObjectState() == Core.DomainObjectState.Existing);
            Assert.IsFalse(customer.GetIsChanged());

            customer.Name = "New Name";

            Assert.IsTrue(customer.GetIsChanged());
        }

        [Test]
        public void TestAggregateChange()
        {
            var repo = new InvoiceRepository();
            var invoice = repo.GetById(1);

            Assert.IsTrue(invoice.GetObjectState() == Core.DomainObjectState.Existing);
            Assert.IsFalse(invoice.GetIsChanged());
            Assert.IsFalse(invoice.GetIsChangedDeep());

            var line = invoice.CreateNewLine();
            line.Quantity = 1;
            line.ProductId = 1;
            

            Assert.IsTrue(invoice.GetIsChangedDeep());

            invoice.AcceptChangesDeep();
            Assert.IsFalse(invoice.GetIsChangedDeep());

            invoice.InvoiceLines.First().Quantity = 3;
            Assert.IsTrue(invoice.GetIsChangedDeep());

            invoice.AcceptChangesDeep();
            Assert.IsFalse(invoice.GetIsChangedDeep());
        }
    }
}
 