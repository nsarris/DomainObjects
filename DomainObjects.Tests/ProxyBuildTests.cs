using DomainObjects.Internal;
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
    class ProxyBuildTests
    {
        [Test]
        public void ProxyTypeBuilderTest()
        {
            var customerProxy = ProxyTypeBuilder.PropertyChangedProxy<Customer>();
            var customer = (Customer)customerProxy.GetConstructorsEx().First().InvokeWithDefaults();
            customer.BeginTracking();
            customer.Name = "12";
            customer.Name = "121";
            customer.Name = "121";
            
            var c = ProxyTypeBuilder.CreateInstance<Customer>(() => new Customer(1, "123"));
            //var c1 = ProxyTypeBuilder.CreateInstance<Customer>(() => new Customer(1, customer.Name));
        }
    }
}
