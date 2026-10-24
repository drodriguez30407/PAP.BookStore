using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Generators
{
    public sealed class ShoppingCartsGenerator
    {
        public GenerationResult Generate(int count)
        {
            return new GenerationService().Generate("ShoppingCarts", count);
        }

        internal List<ShoppingCart> CreateBatch(int count, IList<Customer> customers)
        {
            GeneratorTools.RequireParents(customers.Count, "Clientes");
            var records = new List<ShoppingCart>();

            DateTime now = DateTime.UtcNow;
            for (int i = 0; i < count; i++)
                records.Add(new ShoppingCart
                {
                    CustomerId = customers[i % customers.Count].Id, CreatedAt = now, UpdatedAt = now
                });
            return records;
        }

        internal int GenerateRecords(BookStoreDBEntities context, int count)
        {
            var customers = context.Customers.AsNoTracking().OrderBy(x => x.Id).ToList();
            var batch = CreateBatch(count, customers);
            using (var repository = new ShoppingCartRepository(context))
            {
                repository.AddRange(batch);

            }
            return batch.Count;
        }
    }
}
