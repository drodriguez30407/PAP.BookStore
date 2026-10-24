using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Generators
{
    public sealed class OrderItemsGenerator
    {
        public GenerationResult Generate(int count)
        {
            return new GenerationService().Generate("OrderItems", count);
        }

        internal List<OrderItem> CreateBatch(int count, IList<Order> orders, IList<Book> books)
        {
            GeneratorTools.RequireParents(orders.Count, "Pedidos");
            GeneratorTools.RequireParents(books.Count, "Libros");
            var records = new List<OrderItem>();

            int size = GeneratorTools.PairCount(count, orders.Count, books.Count);
            for (int i = 0; i < size; i++)
            {
                var book = books[i / orders.Count];
                records.Add(new OrderItem
                {
                    OrderId = orders[i % orders.Count].Id, BookId = book.Id,
                    Quantity = 1 + i % 4, UnitPrice = book.Price, Discount = 0m
                });
            }
            return records;
        }

        internal int GenerateRecords(BookStoreDBEntities context, int count)
        {
            var orders = context.Orders.AsNoTracking().OrderBy(x => x.Id).ToList();
            var books = context.Books.AsNoTracking().OrderBy(x => x.Id).ToList();
            var batch = CreateBatch(count, orders, books);
            using (var repository = new OrderItemRepository(context))
            {
                repository.AddRange(batch);
                repository.RecalculateOrderTotals(batch.Select(x => x.OrderId));
            }
            return batch.Count;
        }
    }
}
