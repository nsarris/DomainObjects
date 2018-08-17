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
            var repo = new BookRepository();
            var book = repo.CreateNew();

            Assert.IsTrue(book.GetObjectState() == Core.DomainObjectState.New);
            Assert.IsFalse(book.GetIsChanged());
            
            book.Title = "New Title";

            Assert.IsTrue(book.GetIsChanged());
            //Assert.IsTrue(book.get)
        }

        [Test]
        public void TestPropertyChanges()
        {
            var repo = new BookRepository();
            var book = repo.GetById(1);

            Assert.IsTrue(book.GetObjectState() == Core.DomainObjectState.Existing);
            Assert.IsFalse(book.GetIsChanged());

            book.Title = "New Title";

            Assert.IsTrue(book.GetIsChanged());
            //Assert.IsTrue(book.get)
        }
    }
}
 