using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Generators
{
    public sealed class CartItemsGenerator
    {
        public GenerationResult Generate(int count)
        {
            return new GenerationService().Generate("CartItems", count);
        }

        internal List<CartItem> CreateBatch(int count, IList<ShoppingCart> carts, IList<Book> books)
        {
            GeneratorTools.RequireParents(carts.Count, "Carritos");
            GeneratorTools.RequireParents(books.Count, "Libros");
            var records = new List<CartItem>();

            int size = GeneratorTools.PairCount(count, carts.Count, books.Count);
            DateTime now = DateTime.UtcNow;
            for (int i = 0; i < size; i++)
                records.Add(new CartItem
                {
                    CartId = carts[i % carts.Count].Id, BookId = books[i / carts.Count].Id,
                    Quantity = 1 + i % 4, AddedAt = now
                });
            return records;
        }

        internal int GenerateRecords(BookStoreDBEntities context, int count)
        {
            var carts = context.ShoppingCarts.AsNoTracking().OrderBy(x => x.Id).ToList();
            var books = context.Books.AsNoTracking().OrderBy(x => x.Id).ToList();
            var batch = CreateBatch(count, carts, books);
            using (var repository = new CartItemRepository(context))
            {
                repository.AddRange(batch);

            }
            return batch.Count;
        }
    }
}
