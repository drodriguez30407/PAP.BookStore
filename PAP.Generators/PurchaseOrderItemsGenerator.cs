using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Generators
{
    public sealed class PurchaseOrderItemsGenerator
    {
        public GenerationResult Generate(int count)
        {
            return new GenerationService().Generate("PurchaseOrderItems", count);
        }

        internal List<PurchaseOrderItem> CreateBatch(int count, IList<PurchaseOrder> purchases, IList<Book> books)
        {
            GeneratorTools.RequireParents(purchases.Count, "Órdenes de compra");
            GeneratorTools.RequireParents(books.Count, "Libros");
            var records = new List<PurchaseOrderItem>();

            int size = GeneratorTools.PairCount(count, purchases.Count, books.Count);
            for (int i = 0; i < size; i++)
            {
                var book = books[i / purchases.Count];
                records.Add(new PurchaseOrderItem
                {
                    PurchaseOrderId = purchases[i % purchases.Count].Id, BookId = book.Id,
                    Quantity = 2 + i % 4,
                    UnitCost = Math.Round(book.Price * 0.60m, 2, MidpointRounding.AwayFromZero)
                });
            }
            return records;
        }

        internal int GenerateRecords(BookStoreDBEntities context, int count)
        {
            var purchases = context.PurchaseOrders.AsNoTracking().OrderBy(x => x.Id).ToList();
            var books = context.Books.AsNoTracking().OrderBy(x => x.Id).ToList();
            var batch = CreateBatch(count, purchases, books);
            using (var repository = new PurchaseOrderItemRepository(context))
            {
                repository.AddRange(batch);
                repository.RecalculatePurchaseTotals(batch.Select(x => x.PurchaseOrderId));
            }
            return batch.Count;
        }
    }
}
