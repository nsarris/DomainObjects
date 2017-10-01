using DomainObjects.Core;
using Dynamix;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using System.Linq;
using System.Reflection;
using Dynamix.Reflection;

namespace TestConsoleApp1
{
    class Book : DomainEntity
    {
        //private int test = 1;
        public int ID { get; set; }
        public string Name { get; set; }
    }

    class Author : DomainEntity
    {
        [DomainKey(1)]
        public string FirstName { get; set; }
        [DomainKey(2)]
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public List<Book> Books { get; set; }
        public IList Books2 { get; set; }
        public IList<Book> Books3 { get; set; }
        public ICollection<Book> Books4 { get; set; }
        public IEnumerable<Book> Books5 { get; set; }
        public IReadOnlyCollection<Book> BooksR1 { get; set; }
        public IReadOnlyList<Book> BooksR2 { get; set; }

        public IDictionary<int, Book> BooksD1 { get; set; }
        public Dictionary<string, Book> BooksD2 { get; set; }
        public Book Book { get; set; }

        public int Foo(int a, int b)
        {
            return a + b;
        }

        public void VoidFoo(string name, DateTime dob) { this.FirstName = name; this.DOB = dob; }

        public T GenericFoo<T>(T a, T b)
        {
            return a;
        }

        public void GenericVoidFoo<T>(T a, string name) { }

        public static bool StaticFoo(int a, int b) { return a == b; }

    }

    class Author2 : DomainEntity
    {
        public class AuthorKey : DomainKey
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
        [DomainKey(1)]
        public AuthorKey Key { get; set; }
    }

    class Key1 : DomainKey
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    class Key2 : DomainKey
    {
        public int Id { get; set; }
    }

    class Address : Immutable
    {
        //public Address(string street, int number, List<Phone> phones)
        public Address(string street, int number, IEnumerable<Phone> phones1, IEnumerable<Phone> phones2, IEnumerable<Phone> phones3)
            : base(new object[] { street, number, phones1, phones2, phones3 })
        {
            //this.Street = street;
            //this.Number = number;
            //this.Phones = phones;
        }

        public string Street { get; private set; }
        public int Number { get; private set; }
        public ImmutableList<Phone> Phones1 { get; private set; }
        public ImmutableArray<Phone> Phones2 { get; private set; }
        public ReadOnlyCollection<Phone> Phones3 { get; private set; }
    }

    class Phone : Immutable
    {
        public Phone(int areaCode, int number)
            : base(new[] { areaCode, number })
        {
            //this.AreaCode = areaCode;
            //this.Number = number;
        }
        public int AreaCode { get; private set; }
        public int Number { get; private set; }
    }



    class Program
    {



        static void Main(string[] args)
        {
            //var isFunc1 = typeof(Func<,,>).IsFunc();
            //var isFunc2 = typeof(string).IsFunc();
            //var isAction1 = typeof(Func<,,>).IsAction();
            //var isAction2 = typeof(Action<,,>).IsAction();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            
            var ll = new List<Author>();

            

            var anon = new
            {
                a = 1,
                b = 2
            };

            var anonw = AnonymousTypeWrapper.Create(anon);
            anonw["a"] = 4;
            anonw["b"] = 5;

            var anon2 = ActivatorEx.CreateInstance(anon);
            var anon2w = AnonymousTypeWrapper.Create(anon2);
            anon2w["a"] = 44;
            anon2w["b"] = 55;

            //var arrayDescriptor = EnumerableTypeDescriptor.Get(new int[] { 1, 2 }.GetType());
            //var arrayClassDescriptor = EnumerableTypeDescriptor.Get(typeof(Array));
            //var arrayListDescriptor = EnumerableTypeDescriptor.Get(typeof(ArrayList));

            //var ListOfDescriptor = EnumerableTypeDescriptor.Get(typeof(List<int>));

            //var CollectionOfDescriptor = EnumerableTypeDescriptor.Get(typeof(Collection<int>));

            var dd = new Dictionary<int, string>();
            dd.Add(1, "2");
            dd.Add(2, "3");
            var DictionaryDescriptor = EnumerableTypeDescriptor.Get(typeof(Dictionary<int, string>));
            var t1 = DictionaryDescriptor.ToArray(dd);
            var t2 = DictionaryDescriptor.ToArray<KeyValuePair<int, string>>(dd);
            var t3 = DictionaryDescriptor.ToList(dd);
            var t4 = DictionaryDescriptor.ToList<KeyValuePair<int, string>>(dd);
            var t5 = DictionaryDescriptor.ToReadOnlyCollection<KeyValuePair<int, string>>(dd);

            var b = new Book { ID = 1 };


            var a11 = new Author
            {
                FirstName = "Nikos",
                LastName = "Sarris",
                DOB = new DateTime(1979, 3, 26),
                Book = new Book { ID = 3, Name = "Somebok" },
            };
            a11.Books = new List<Book>(new Book[] { new Book { ID = 1 }, new Book { ID = 2 } });
            a11.Books2 = new List<Book>(new Book[] { new Book { ID = 11 }, new Book { ID = 12 } });
            a11.Books3 = new List<Book>(new Book[] { new Book { ID = 21 }, new Book { ID = 22 } });
            a11.Books4 = new List<Book>(new Book[] { new Book { ID = 31 }, new Book { ID = 32 } });
            a11.Books5 = new List<Book>(new Book[] { new Book { ID = 31 }, new Book { ID = 32 } });

            a11.BooksR1 = new List<Book>(new Book[] { new Book { ID = 31 }, new Book { ID = 32 } });
            a11.BooksR2 = new List<Book>(new Book[] { new Book { ID = 31 }, new Book { ID = 32 } });


            var a12 = new Author { FirstName = "Nikos", LastName = "Sarris", DOB = new DateTime(1979, 3, 26) };
            a12.Books = new List<Book>(new Book[] { new Book { ID = 1 }, new Book { ID = 2 } });
            var a13 = new Author { FirstName = "Nikos1", LastName = "Sarris", DOB = new DateTime(1979, 3, 26) };
            a13.Books = new List<Book>(new Book[] { new Book { ID = 3 }, new Book { ID = 2 } });

            a11.FirstName = "asdas";

            var cc = ObjectCloner.Clone(a11);

            var a2 = new Author2 { Key = new Author2.AuthorKey { FirstName = "Nikos", LastName = "Sarris" } };
            var bkey = DomainEntityKeyProvider.GetKey(b);
            var a11key = DomainEntityKeyProvider.GetKey(a11);
            var a12key = DomainEntityKeyProvider.GetKey(a12);
            var a13key = DomainEntityKeyProvider.GetKey(a13);
            var a2key = DomainEntityKeyProvider.GetKey(a2);

            var k11 = new Key1 { FirstName = "Nikos", LastName = "Sarris" };
            var k12 = new Key1 { FirstName = "Nikos", LastName = "Sarris" };
            var k13 = new Key1 { FirstName = "Nikos1", LastName = "Sarris" };


            var k21 = new Key2 { Id = 1 };
            var k22 = new Key2 { Id = 1 };
            var k23 = new Key2 { Id = 2 };

            var comparer = new ObjectComparer();
            var r13 = comparer.DeepEquals(a11, a12);
            var r23 = comparer.DeepEquals(a11, a13);
            var r34 = comparer.DeepEquals(a12, a13);


            var ad1 = new Address("Akakion", 35, null, null, null);

            var ad = new Address("Akakion", 35,
                new Phone[] { new Phone(2541, 023110), new Phone(2310, 855006) },
                new Phone[] { new Phone(2541, 023111), new Phone(2310, 855007) },
                new Phone[] { new Phone(2541, 023112), new Phone(2310, 855008) }
                );

            var ad2 = new Mutator<Address>(ad)
                //.Set(x => x.Number, 10)
                //.Set(x => x.Street, "dsada")
                .Set(x => k11.FirstName, "sda")
                .Set(x => x.Phones1[1].Number, 32321)
                //.Add(x => x.Phones2, new Phone(12, 12))
                //.Remove(x => x.Phones2, ad.Phones2[1])
                //.Remove(x => x.Phones2, 2)
                //.Clear(x => x.Phones2)
                .ToImmutable();
        }
    }
}
