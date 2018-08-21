using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using DomainObjects;
using DomainObjects.Core;
using DomainObjects.ModelBuilder;
using DomainObjects.Internal;

namespace DomainObjects.Tests
{
    [TestFixture]
    public class ModelBuilderTests
    {
        [Test]
        public void Test()
        {
            var props = ExpressionHelper.GetSelectedProperties((Author x) => new
            {
                A = x.FirstName,
                B = x.LastName,
                //T = x.FirstName + x.LastName,
            });
        }

        [Test]
        public void TestBuilder()
        {
            var builder = new DomainModelBuilder();
            var bookBuilder = builder.ForEntity<Book>();

            bookBuilder
                //.HasKey(x => x.Isbn)
                //.HasKey(x => new { x.Isbn })
                //.HasKey(x => new { Id = x.Isbn + x.Title })
                .IgnoreMember(x => x.PublishInfo);

            bookBuilder
                .Property(x => x.Type).IsRequired()
                ;

            bookBuilder
                .Property(x => x.PublishInfo)
                ;

            var authorBuilder = builder.ForEntity<Author>();

            //authorBuilder.HasKey(x => x.DOB);

            authorBuilder.Aggregate(x => x.LastPublishedBook).IsRequired();
            authorBuilder.Aggregate(x => x.Books).IsRequired();
        }
    }
}
