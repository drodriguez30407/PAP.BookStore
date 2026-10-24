using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Generators
{
    public sealed class PaymentsGenerator
    {
        public GenerationResult Generate(int count)
        {
            return new GenerationService().Generate("Payments", count);
        }

        internal List<Payment> CreateBatch(int count, IList<Order> orders)
        {
            GeneratorTools.RequireParents(orders.Count, "Pedidos");
            var records = new List<Payment>();

            string key = GeneratorTools.Key();
            DateTime now = DateTime.UtcNow;
            for (int i = 0; i < Math.Min(count, orders.Count); i++)
                records.Add(new Payment
                {
                    OrderId = orders[i].Id, PaymentMethod = "Card", Amount = orders[i].Total,
                    PaymentDate = now, Status = "Completed", TransactionRef = "DEMO-" + key + "-" + (i + 1)
                });
            return records;
        }

        internal int GenerateRecords(BookStoreDBEntities context, int count)
        {
            var orders = context.Orders.AsNoTracking().Where(x => x.Total > 0m && x.OrderItems.Any()).OrderBy(x => x.Id).ToList();
            var batch = CreateBatch(count, orders);
            using (var repository = new PaymentRepository(context))
            {
                repository.AddRange(batch);
                repository.MarkOrdersPaid(batch.Select(x => x.OrderId));
            }
            return batch.Count;
        }
    }
}
