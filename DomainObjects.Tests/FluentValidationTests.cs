using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainObjects.ModelBuilder;
using DomainObjects.Tests.Sales;
using FluentValidation;
using FluentValidation.Resources;
using NUnit.Framework;

namespace DomainObjects.Tests
{
    [TestFixture]
    class FluentValidationTests
    {
        [Test]
        public void BasicValidationTest()
        {
            HackLanguageManager.AddLanguage(new GreekLanguage());
            ValidatorOptions.LanguageManager.Culture = new System.Globalization.CultureInfo("el");

            var repo = new CustomerRepository();

            var customer = repo.CreateNew();
            customer.MainAddress = new Address("Street", "Number", null, null, new Phone("1234", 0),
                new List<Phone>()
                {
                    new Phone("123", 0),
                    new Phone("456", 0),
                    new Phone("", 0),
                });
        }
    }
}
