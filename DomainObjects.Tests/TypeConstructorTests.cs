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
        [Test]
        public void DomainTypeConstructorTest()
        {

            var ctor1 = new DomainEntityFactory<Customer, int, string>();

            var id = 1;
            var name = "someone";

            var c1 = ctor1.Construct(new object[] { id, name });
            Assert.IsTrue(c1.Id == id && c1.Name == name && c1.GetEntityState() == EntityState.Uninitialized);

            c1 = ctor1.New.Construct(("id", id), ("name", name));
            Assert.IsTrue(c1.Id == id && c1.Name == name && c1.GetEntityState() == EntityState.New);

            c1 = ctor1.Existing.Construct(1, name);
            Assert.IsTrue(c1.Id == id && c1.Name == name && c1.GetEntityState() == EntityState.Existing);

            c1 = ctor1.ConstructWithDefaults(("id", id));
            Assert.IsTrue(c1.Id == id && c1.Name == null && c1.GetEntityState() == EntityState.Uninitialized);

            Assert.Throws<InvalidOperationException>(() => c1 = ctor1.Construct(("id", id)));
        }

        [Test]
        public void TypeConstructorTest()
        {

            var ctor1 = new TypeConstructor<Customer, int, string>(ProxyTypeBuilder.BuildPropertyChangedProxy<Customer>(), (Customer x) => x.InitExisting());

            var id = 1;
            var name = "someone";

            var c1 = ctor1.Construct(new object[] { id, name });
            Assert.IsTrue(c1.Id == id && c1.Name == name && c1.GetEntityState() == EntityState.Existing) ;

            c1 = ctor1.Construct(("id" , id), ("name" , name));
            Assert.IsTrue(c1.Id == id && c1.Name == name && c1.GetEntityState() == EntityState.Existing);

            c1 = ctor1.Construct(1, name);
            Assert.IsTrue(c1.Id == id && c1.Name == name && c1.GetEntityState() == EntityState.Existing);

            c1 = ctor1.ConstructWithDefaults(("id", id));
            Assert.IsTrue(c1.Id == id && c1.Name == null && c1.GetEntityState() == EntityState.Existing);

            Assert.Throws<InvalidOperationException>(() => c1 = ctor1.Construct(("id", id)));
        }
    }
}
