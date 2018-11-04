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

            modelBuilder.Entity<Invoice>().HasKey(x => x.Id);
            modelBuilder.Entity<InvoiceLine>().HasKey(x => x.Id);
            modelBuilder.Entity<Product>().HasKey(x => x.Id);

            modelBuilder.Entity<Person>().HasKey(x => x.Id);
            modelBuilder.Entity<Customer>().HasKey(x => x.Id)
                .IgnoreMember(x => x.StringComparer);

            var model = modelBuilder.Build();
        }
    }
}
