using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Generators
{
    public sealed class CustomersGenerator
    {
        public GenerationResult Generate(int count)
        {
            return new GenerationService().Generate("Customers", count);
        }

        internal List<Customer> CreateBatch(int count)
        {

            var records = new List<Customer>();

            string key = GeneratorTools.Key();
            string hash = GeneratorTools.PasswordHash();
            DateTime now = DateTime.UtcNow;
            for (int i = 1; i <= count; i++)
                records.Add(new Customer
                {
                    FirstName = "Cliente " + i, LastName = "Demostración " + key,
                    Email = "cliente" + key + i + "@example.com", PasswordHash = hash,
                    Phone = "+506 8000-" + (1000 + i), BirthDate = DateTime.Today.AddYears(-25).AddDays(-i),
                    IsActive = true, CreatedAt = now, UpdatedAt = now
                });
            return records;
        }

        internal int GenerateRecords(BookStoreDBEntities context, int count)
        {

            var batch = CreateBatch(count);
            using (var repository = new CustomerRepository(context))
            {
                repository.AddRange(batch);

            }
            return batch.Count;
        }
    }
}
