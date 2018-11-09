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
    public class EqualityTests
    {
        [Test]
        public void TestEntityKeyEquality()
        {
            var repo = new CustomerRepository();
            var customer1 = repo.CreateNew();
            var customer2 = repo.GetById(1);

            Assert.IsFalse(customer1.KeyEquals(customer2));
            Assert.IsFalse(customer1.GetKey() == customer2.GetKey());

            customer1.SetKey(1);

            Assert.IsTrue(customer1.KeyEquals(customer2));
            Assert.IsTrue(customer1.GetKey() == customer2.GetKey());
        }
    }
}
