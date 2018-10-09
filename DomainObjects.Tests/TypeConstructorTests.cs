using DomainObjects.Internal;
using DomainObjects.Repositories;
using DomainObjects.Tests.Sales;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Tests
{
    class DomainTypeConstructor<T, TArgument> : TypeConstructor<T, TArgument>
        where T : Core.DomainEntity
    {
        public DomainTypeConstructor()
            :base(x => x.InitNew(true))
        {

        }
    }

    [TestFixture]
    class TypeConstructorTests
    {
        [Test]
        public void TypeConstructorTest()
        {
            var ctor1 = new TypeConstructor<Customer, int, string>();

            var id = 1;
            var name = "someone";

            var c1 = ctor1.Construct(new object[] { id, name });
            Assert.IsTrue(c1.Id == id && c1.Name == name);

            c1 = ctor1.Construct(("id" , id), ("name" , name));
            Assert.IsTrue(c1.Id == id && c1.Name == name);

            c1 = ctor1.Construct(1, name);
            Assert.IsTrue(c1.Id == id && c1.Name == name);

            c1 = ctor1.ConstructWithDefaults(("id", id));
            Assert.IsTrue(c1.Id == id && c1.Name == null);

            Assert.Throws<InvalidOperationException>(() => c1 = ctor1.Construct(("id", id)));
        }
    }
}
