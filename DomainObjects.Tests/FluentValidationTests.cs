using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainObjects.ModelBuilder;
using DomainObjects.Tests.Sales;
using NUnit.Framework;
using Unity;

namespace DomainObjects.Tests
{
    [TestFixture]
    class FluentValidationTests
    {
        [Test]
        public void BasicValidationTest()
        {
            var container = new UnityContainer();
            //container.RegisterInstance(new TestService());
            //container.RegisterInstance(new TestService2());

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
            customer.MainAddress = new Address("Street", "Number", null, null, new Phone("1234", 0),
                new List<Phone>()
                {
                    new Phone("123", 0),
                    new Phone("456", 0),
                    new Phone("", 0),
                });

            var customerValidator = container.Resolve<Customer.Validator>();
            var validationResult = customerValidator.Validate(customer);
        }
    }
}
