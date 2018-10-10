using DomainObjects.Core;
using DomainObjects.Internal;
using DomainObjects.ModelBuilder;
using DomainObjects.Repositories;
using DomainObjects.Tests.Sales;
using Dynamix.Reflection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Tests
{
    
    

    [TestFixture]
    class TypeConstructorTests
    {
        [OneTimeSetUp]
        public void Setup()
        {
            var modelBuilder = new DomainModelBuilder()
                .HasModelName("Sales");

            var invoiceBuilder = modelBuilder.Entity<Invoice>().HasKey(x => x.Id);
            var invoiceLineBuilder = modelBuilder.Entity<InvoiceLine>().HasKey(x => x.Id);
            var productBuilder = modelBuilder.Entity<Product>().HasKey(x => x.Id);
            var customerBuilder = modelBuilder.Entity<Customer>().HasKey(x => new { x.Id }).IgnoreMember(x => x.StringComparer);

            modelBuilder.Build();
        }

        [Test]
        public void DomainTypeConstructorTest()
        {

            var ctor1 = new DomainEntityFactory<Customer, int, string>();

            var id = 1;
            var name = "someone";

            var c1 = ctor1.Construct(new object[] { id, name });
            Assert.IsTrue(c1.Id == id && c1.Name == name && c1.GetObjectState() == DomainObjectState.Uninitialized);

            c1 = ctor1.New.Construct(("id", id), ("name", name));
            Assert.IsTrue(c1.Id == id && c1.Name == name && c1.GetObjectState() == DomainObjectState.New);

            c1 = ctor1.Existing.Construct(1, name);
            Assert.IsTrue(c1.Id == id && c1.Name == name && c1.GetObjectState() == DomainObjectState.Existing);

            c1 = ctor1.ConstructWithDefaults(("id", id));
            Assert.IsTrue(c1.Id == id && c1.Name == null && c1.GetObjectState() == DomainObjectState.Uninitialized);

            Assert.Throws<InvalidOperationException>(() => c1 = ctor1.Construct(("id", id)));
        }

        [Test]
        public void TypeConstructorTest()
        {

            var ctor1 = new TypeConstructor<Customer, int, string>(ProxyTypeBuilder.BuildPropertyChangedProxy<Customer>(), (Customer x) => x.InitExisting());

            var id = 1;
            var name = "someone";

            var c1 = ctor1.Construct(new object[] { id, name });
            Assert.IsTrue(c1.Id == id && c1.Name == name && c1.GetObjectState() == DomainObjectState.Existing) ;

            c1 = ctor1.Construct(("id" , id), ("name" , name));
            Assert.IsTrue(c1.Id == id && c1.Name == name && c1.GetObjectState() == DomainObjectState.Existing);

            c1 = ctor1.Construct(1, name);
            Assert.IsTrue(c1.Id == id && c1.Name == name && c1.GetObjectState() == DomainObjectState.Existing);

            c1 = ctor1.ConstructWithDefaults(("id", id));
            Assert.IsTrue(c1.Id == id && c1.Name == null && c1.GetObjectState() == DomainObjectState.Existing);

            Assert.Throws<InvalidOperationException>(() => c1 = ctor1.Construct(("id", id)));
        }
    }
}
