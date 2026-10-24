using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Generators
{
    public sealed class ReviewsGenerator
    {
        public GenerationResult Generate(int count)
        {
            return new GenerationService().Generate("Reviews", count);
        }

        internal List<Review> CreateBatch(int count, IList<Book> books, IList<Customer> customers)
        {
            GeneratorTools.RequireParents(books.Count, "Libros");
            GeneratorTools.RequireParents(customers.Count, "Clientes");
            var records = new List<Review>();

            int size = GeneratorTools.PairCount(count, books.Count, customers.Count);
            DateTime now = DateTime.UtcNow;
            for (int i = 0; i < size; i++)
                records.Add(new Review
                {
                    BookId = books[i % books.Count].Id, CustomerId = customers[i / books.Count].Id,
                    Rating = (byte)(1 + i % 5), Comment = "Reseña de demostración " + (i + 1), CreatedAt = now
                });
            return records;
        }

        internal int GenerateRecords(BookStoreDBEntities context, int count)
        {
            var books = context.Books.AsNoTracking().OrderBy(x => x.Id).ToList();
            var customers = context.Customers.AsNoTracking().OrderBy(x => x.Id).ToList();
            var batch = CreateBatch(count, books, customers);
            using (var repository = new ReviewRepository(context))
            {
                repository.AddRange(batch);

            }
            return batch.Count;
        }
    }
}
