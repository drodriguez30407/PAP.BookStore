using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Generators
{
    public sealed class AddressesGenerator
    {
        public GenerationResult Generate(int count)
        {
            return new GenerationService().Generate("Addresses", count);
        }

        internal List<Address> CreateBatch(int count, IList<Customer> customers)
        {
            GeneratorTools.RequireParents(customers.Count, "Clientes");
            var records = new List<Address>();

            for (int i = 0; i < count; i++)
                records.Add(new Address
                {
                    CustomerId = customers[i % customers.Count].Id, AddressType = "Shipping",
                    Street = "Dirección de prueba " + (i + 1), City = "Heredia", State = "Heredia",
                    PostalCode = "40101", Country = "Costa Rica", IsDefault = i < customers.Count
                });
            return records;
        }

        internal int GenerateRecords(BookStoreDBEntities context, int count)
        {
            var customers = context.Customers.AsNoTracking().OrderBy(x => x.Id).ToList();
            var batch = CreateBatch(count, customers);
            using (var repository = new AddressRepository(context))
            {
                repository.AddRange(batch);

            }
            return batch.Count;
        }
    }
}
