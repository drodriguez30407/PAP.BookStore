using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Generators
{
    public sealed class OrdersGenerator
    {
        public GenerationResult Generate(int count)
        {
            return new GenerationService().Generate("Orders", count);
        }

        internal List<Order> CreateBatch(int count, IList<Address> addresses)
        {
            GeneratorTools.RequireParents(addresses.Count, "Direcciones");
            var records = new List<Order>();

            DateTime now = DateTime.UtcNow;
            for (int i = 0; i < count; i++)
            {
                var address = addresses[i % addresses.Count];
                records.Add(new Order
                {
                    CustomerId = address.CustomerId, ShippingAddressId = address.Id,
                    OrderDate = now, Status = "Pending", Subtotal = 0m, TaxAmount = 0m,
                    ShippingCost = 0m, Total = 0m, Notes = "Pedido de demostración", UpdatedAt = now
                });
            }
            return records;
        }

        internal int GenerateRecords(BookStoreDBEntities context, int count)
        {
            var addresses = context.Addresses.AsNoTracking().OrderBy(x => x.Id).ToList();
            var batch = CreateBatch(count, addresses);
            using (var repository = new OrderRepository(context))
            {
                repository.AddRange(batch);

            }
            return batch.Count;
        }
    }
}
