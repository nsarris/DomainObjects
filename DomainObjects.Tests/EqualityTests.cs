using DomainObjects.Tests.Books;
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
        public void TestMethod()
        {
            var book1 = new Book(1, "Test1");
            var book2 = new Book(1, "Test2");
            var book3 = new Book(2, "Test2");

            //var author = new Author();

            Assert.AreEqual(book1, book2);
            Assert.AreNotEqual(book1, book3);
        }
    }
}
