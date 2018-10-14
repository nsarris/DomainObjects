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
    [SetUpFixture]
    class Setup
    {
        [OneTimeSetUp]
        public void CommonModelSetup()
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
    }
}
