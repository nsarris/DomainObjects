﻿using DomainObjects.ModelBuilder;
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

            customer.MainAddress = new Address("", "", "", "", null, null);
            customer.AcceptChangesDeep();

            Assert.IsFalse(customer.GetIsChanged());

            customer.MainAddress = new Address("", "", "", "", null, null);

            //var a1 = new Address("", "", "", "", null, null);
            //var a2 = new Address("", "", "", "", null, null);

            //Assert.IsTrue(a1 == a2);
            //Assert.IsTrue(a1.Equals(a2));
            //Assert.IsTrue(object.Equals(a1, a2));
            //Assert.IsTrue(EqualityComparer<Address>.Default.Equals(a1, a2));
            //Assert.IsFalse(object.ReferenceEquals(a1, a2));

            Assert.IsFalse(customer.GetIsChanged());
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
 