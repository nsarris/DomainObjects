﻿using DomainObjects.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;

namespace DomainObjects.Tests.Books
{
    class BookRepository
    {

        public Book CreateNew()
        {
            var book = new Book(0, null, BookType.Digital);
            book.MarkNew();
            book.BeginTracking();
            return book;
        }

        public Book GetById(int id)
        {
            var book = new Book(id, "Title", BookType.Digital);
            book.MarkExisting();
            book.BeginTracking();
            return book;
        }
    }

    enum BookType
    {
        Digital,
        HarCopy
    }

    class PublishInfo : DomainValue
    {
        public PublishInfo(string publisher, DateTime publishDate)
        {
            Publisher = publisher;
            PublishDate = publishDate;
        }

        public string Publisher { get; set; }
        public DateTime PublishDate { get; set; }
    }

    [AddINotifyPropertyChangedInterface]
    class Book : Aggregate
    {
        public Book(int isbn, string title, BookType bookType = BookType.HarCopy, PublishInfo publishInfo = null)
        {
            Isbn = isbn;
            Title = title;
            Type = bookType;
            this.PublishInfo = publishInfo;
            this.MarkExisting();
            this.BeginTracking();
        }

        [DomainKey(1)]
        public int Isbn { get; }
        public string Title { get; set; }
        public BookType Type { get; set; }
        public PublishInfo PublishInfo { get; set; }
    }

    [AddINotifyPropertyChangedInterface]
    class Author : DomainEntity
    {
        [DomainKey(1)]
        public string FirstName { get; set; }
        [DomainKey(2)]
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public Book LastPublishedBook { get; set; }
        public AggregateList<Book> Books { get; set; }
    }

    [AddINotifyPropertyChangedInterface]
    class Author2 : DomainEntity
    {
        public class AuthorKey : DomainKey
        {
            public AuthorKey(string firstName, string lastName)
            {
                FirstName = firstName;
                LastName = lastName;
            }

            public string FirstName { get; }
            public string LastName { get; }
        }
        [DomainKey(1)]
        public AuthorKey Key { get; set; }

        public DateTime DOB { get; set; }
        public TrackableList<Book> Books { get; set; }
    }

    class Key1 : DomainKey
    {
        public Key1(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string FirstName { get; }
        public string LastName { get; }

    }

    class Key2 : DomainKey
    {
        public Key2(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }

    class Address //: Immutable
    {
        public Address(string street, int number, IEnumerable<Phone> phones)
        //: base(new object[] { street, number, phones })
        {
            this.Street = street;
            this.Number = number;
            this.Phones = new TrackableReadOnlyList<Phone>(phones);
        }

        public string Street { get; private set; }
        public int Number { get; private set; }
        public IReadOnlyList<Phone> Phones { get; private set; }
    }

    class Phone //: Immutable
    {
        public Phone(int areaCode, int number)
        //: base(new[] { areaCode, number })
        {
            this.AreaCode = areaCode;
            this.Number = number;
        }
        public int AreaCode { get; private set; }
        public int Number { get; private set; }
    }
}
