using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Generators
{
    public sealed class BookAuthorsGenerator
    {
        public GenerationResult Generate(int count)
        {
            return new GenerationService().Generate("BookAuthors", count);
        }

        internal List<BookAuthorLink> CreateBatch(int count, IList<Book> books, IList<Author> authors)
        {
            GeneratorTools.RequireParents(books.Count, "Libros");
            GeneratorTools.RequireParents(authors.Count, "Autores");
            var records = new List<BookAuthorLink>();

            int size = GeneratorTools.PairCount(count, books.Count, authors.Count);
            for (int i = 0; i < size; i++)
                records.Add(new BookAuthorLink
                {
                    BookId = books[i % books.Count].Id, AuthorId = authors[i / books.Count].Id
                });
            return records;
        }

        internal int GenerateRecords(BookStoreDBEntities context, int count)
        {
            var books = context.Books.AsNoTracking().OrderBy(x => x.Id).ToList();
            var authors = context.Authors.AsNoTracking().OrderBy(x => x.Id).ToList();
            var batch = CreateBatch(count, books, authors);
            using (var repository = new BookAuthorRepository(context))
            {
                repository.AddRange(batch);

            }
            return batch.Count;
        }
    }
}
