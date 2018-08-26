using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainObjects.ModelBuilder;
using DomainObjects.Tests.Sales;
using NUnit.Framework;

namespace DomainObjects.Tests
{
    [TestFixture]
    class FluentValidationTests
    {
        [Test]
        public void BasicValidationTest()
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

            var repo = new CustomerRepository();

            var customer = repo.CreateNew();
            customer.MainAddress = new Address("Street", "Number", null, null, null, null);

            var validationResult = new Customer.Validator(new Customer.TestService()).Validate(customer);
        }
    }
}
