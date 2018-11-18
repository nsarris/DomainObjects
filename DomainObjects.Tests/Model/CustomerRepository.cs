using DomainObjects.Core;
using DomainObjects.Repositories;
using Dynamix.Helpers;
using Dynamix.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DomainObjects.Tests.Sales
{
    public class CustomerRepository : IEntityQueryProvider<Customer, int>, IEntityFactory<Customer>
    {
        DomainEntityFactory<Customer, int, string> factory = new DomainEntityFactory<Customer, int, string>();

        //Repo default

        public bool SupportsAsync => false;

        public bool SupportsQueryable => false;

        public Customer Create(params object[] parameters)
        {
            return factory.New.Construct(parameters);
        }

        public Customer Create(params (string name, object value)[] namedValues)
        {
            return factory.New.Construct(namedValues);
        }

        public Customer Create(IEnumerable<object> parameters)
        {
            return factory.New.Construct(parameters);
        }

        public Customer Create(IEnumerable<(string name, object value)> namedValues)
        {
            return factory.New.Construct(namedValues);
        }

        
        public Customer GetById(int id)
        {
            return factory.Existing.ConstructWithDefaults(("id", id));
        }

        public Customer GetById(object id)
        {
            return GetById(ConvertEx.ConvertTo(id, typeof(int)));
        }

        public Task<Customer> GetByIdAsync(object id)
        {
            return Task.FromResult(GetById(id));
        }

        public Task<Customer> GetByIdAsync(int id)
        {
            return GetByIdAsync(id);
        }

        public SingleQueryable<Customer> QueryById(object id)
        {
            return new SingleQueryable<Customer>(GetById(id));
        }

        public SingleQueryable<Customer> QueryById(int id)
        {
            return new SingleQueryable<Customer>(GetById(id));
        }

        public IQueryable<Customer> ToQueryable()
        {
            throw new NotSupportedException();
        }

        //Custom

        public Customer Create()
        {
            return factory.New.Construct(0, null);
        }

        public Customer CreateNew(int id, string name)
        {
            return factory.New.Construct(id, name);
        }
    }
}
