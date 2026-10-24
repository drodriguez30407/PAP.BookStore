using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Generators
{
    public sealed class PurchaseOrdersGenerator
    {
        public GenerationResult Generate(int count)
        {
            return new GenerationService().Generate("PurchaseOrders", count);
        }

        internal List<PurchaseOrder> CreateBatch(int count, IList<Supplier> suppliers)
        {
            GeneratorTools.RequireParents(suppliers.Count, "Proveedores");
            var records = new List<PurchaseOrder>();

            DateTime now = DateTime.UtcNow;
            for (int i = 0; i < count; i++)
                records.Add(new PurchaseOrder
                {
                    SupplierId = suppliers[i % suppliers.Count].Id, OrderDate = now,
                    ExpectedDate = DateTime.Today.AddDays(7), Status = "Pending", TotalCost = 0m
                });
            return records;
        }

        internal int GenerateRecords(BookStoreDBEntities context, int count)
        {
            var suppliers = context.Suppliers.AsNoTracking().OrderBy(x => x.Id).ToList();
            var batch = CreateBatch(count, suppliers);
            using (var repository = new PurchaseOrderRepository(context))
            {
                repository.AddRange(batch);

            }
            return batch.Count;
        }
    }
}
