using DomainObjects.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainObjects.Repositories;
using Dynamix.QueryableExtensions;
using Dynamix.Helpers;

namespace DomainObjects.Tests.Sales
{
    public class InvoiceRepository : IEntityQueryProvider<Invoice, int>, IEntityFactory<Invoice>
    {
        DomainEntityFactory<Invoice, int> factory = new DomainEntityFactory<Invoice, int>();

        //Repo default

        public bool SupportsAsync => false;

        public bool SupportsQueryable => false;

        public Invoice Create(params object[] parameters)
        {
            return factory.New.Construct(parameters);
        }

        public Invoice Create(params (string name, object value)[] namedValues)
        {
            return factory.New.Construct(namedValues);
        }

        public Invoice Create(IEnumerable<object> parameters)
        {
            return factory.New.Construct(parameters);
        }

        public Invoice Create(IEnumerable<(string name, object value)> namedValues)
        {
            return factory.New.Construct(namedValues);
        }

        public Invoice GetById(int id)
        {
            return factory.Existing.Construct(id);
        }

        public Invoice GetById(object id)
        {
            return GetById(ConvertEx.ConvertTo(id, typeof(int)));
        }

        public Task<Invoice> GetByIdAsync(object id)
        {
            return Task.FromResult(GetById(id));
        }

        public Task<Invoice> GetByIdAsync(int id)
        {
            return GetByIdAsync(id);
        }

        public SingleQueryable<Invoice> QueryById(object id)
        {
            return new SingleQueryable<Invoice>(GetById(id));
        }

        public SingleQueryable<Invoice> QueryById(int id)
        {
            return new SingleQueryable<Invoice>(GetById(id));
        }

        public IQueryable<Invoice> ToQueryable()
        {
            throw new NotSupportedException();
        }

        //Custom

        public Invoice Create()
        {
            return factory.New.Construct(0);
        }
    }
}
