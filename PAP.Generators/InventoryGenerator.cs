using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Generators
{
    public sealed class InventoryGenerator
    {
        public GenerationResult Generate(int count)
        {
            return new GenerationService().Generate("Inventory", count);
        }

        internal List<Inventory> CreateBatch(int count, IList<Book> books)
        {
            GeneratorTools.RequireParents(books.Count, "Libros");
            var records = new List<Inventory>();

            DateTime now = DateTime.UtcNow;
            for (int i = 0; i < Math.Min(count, books.Count); i++)
                records.Add(new Inventory
                {
                    BookId = books[i].Id, QuantityInStock = 50 + i, ReorderLevel = 5, LastRestockedAt = now
                });
            return records;
        }

        internal int GenerateRecords(BookStoreDBEntities context, int count)
        {
            var books = context.Books.AsNoTracking().OrderBy(x => x.Id).ToList();
            var batch = CreateBatch(count, books);
            using (var repository = new InventoryRepository(context))
            {
                repository.AddRange(batch);

            }
            return batch.Count;
        }
    }
}
