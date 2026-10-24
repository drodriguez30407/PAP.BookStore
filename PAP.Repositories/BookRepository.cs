using System;
using System.Collections.Generic;
using System.Linq;
using PAP.DataAccess;

namespace PAP.Repositories
{
    public class BookRepository : EntityRepository<Book>
    {
        public BookRepository() : base(x => x.Id, CopyChanges) { }
        public BookRepository(BookStoreDBEntities context) : base(context, x => x.Id, CopyChanges) { }

        private static void CopyChanges(Book target, Book source)
        {
            target.Title = source.Title;
            target.Description = source.Description;
            target.PublicationDate = source.PublicationDate;
            target.Price = source.Price;
            target.UpdatedAt = DateTime.UtcNow;
            target.PublisherId = source.PublisherId;
            target.CategoryId = source.CategoryId;
            target.GenreId = source.GenreId;
            target.ISBN = source.ISBN;
            target.Pages = source.Pages;
            target.Language = source.Language;
            target.CoverImageUrl = source.CoverImageUrl;
        }

    }
}
