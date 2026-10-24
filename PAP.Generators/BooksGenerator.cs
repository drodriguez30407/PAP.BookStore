using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Generators
{
    public sealed class BooksGenerator
    {
        public GenerationResult Generate(int count)
        {
            return new GenerationService().Generate("Books", count);
        }

        internal List<Book> CreateBatch(int count, IList<Category> categories, IList<Genre> genres, IList<Publisher> publishers)
        {
            GeneratorTools.RequireParents(categories.Count, "Categorías");
            GeneratorTools.RequireParents(genres.Count, "Géneros");
            GeneratorTools.RequireParents(publishers.Count, "Editoriales");
            var records = new List<Book>();

            string key = GeneratorTools.Key();
            string isbnPrefix = GeneratorTools.IsbnPrefix();
            DateTime now = DateTime.UtcNow;
            for (int i = 0; i < count; i++)
            {
                records.Add(new Book
                {
                    Title = "Libro de prueba " + (i + 1) + " " + key,
                    Description = "Libro generado para demostrar las relaciones del catálogo.",
                    PublicationDate = DateTime.Today.AddYears(-1).AddDays(-i),
                    Price = 1000m + 125m * i, CreatedAt = now, UpdatedAt = now,
                    CategoryId = categories[i % categories.Count].Id,
                    GenreId = genres[i % genres.Count].Id,
                    PublisherId = publishers[i % publishers.Count].Id,
                    ISBN = GeneratorTools.Isbn(isbnPrefix, i + 1), Pages = 100 + i,
                    Language = "Español", CoverImageUrl = null
                });
            }
            return records;
        }

        internal int GenerateRecords(BookStoreDBEntities context, int count)
        {
            var categories = context.Categories.AsNoTracking().OrderBy(x => x.Id).ToList();
            var genres = context.Genres.AsNoTracking().OrderBy(x => x.Id).ToList();
            var publishers = context.Publishers.AsNoTracking().OrderBy(x => x.Id).ToList();
            var batch = CreateBatch(count, categories, genres, publishers);
            using (var repository = new BookRepository(context))
            {
                repository.AddRange(batch);

            }
            return batch.Count;
        }
    }
}
