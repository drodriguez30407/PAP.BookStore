using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PAP.DataAccess;

namespace PAP.Repositories
{
    public class BookRepository : EntityRepository<Book>
    {
        public BookRepository() : base(x => x.Id, CopyChanges) { }
        public BookRepository(BookStoreDBEntities context) : base(context, x => x.Id, CopyChanges) { }

        public List<Book> GetAllWithDetails()
        {
            return Context.Books.AsNoTracking()
                .Include(x => x.Category).Include(x => x.Genre).Include(x => x.Publisher)
                .OrderBy(x => x.Title).ThenBy(x => x.Id).ToList();
        }

        public Book GetWithDetails(int id)
        {
            return Context.Books.Include(x => x.Category).Include(x => x.Genre)
                .Include(x => x.Publisher).SingleOrDefault(x => x.Id == id);
        }

        public List<Category> GetCategories()
        {
            return Context.Categories.AsNoTracking().OrderBy(x => x.Name).ThenBy(x => x.Id).ToList();
        }

        public List<Genre> GetGenres()
        {
            return Context.Genres.AsNoTracking().OrderBy(x => x.Name).ThenBy(x => x.Id).ToList();
        }

        public List<Publisher> GetPublishers()
        {
            return Context.Publishers.AsNoTracking().OrderBy(x => x.Name).ThenBy(x => x.Id).ToList();
        }

        public bool CategoryExists(int id) { return Context.Categories.Any(x => x.Id == id); }
        public bool GenreExists(int id) { return Context.Genres.Any(x => x.Id == id); }
        public bool PublisherExists(int id) { return Context.Publishers.Any(x => x.Id == id); }

        public bool IsbnExists(string isbn, int? excludeId = null)
        {
            var query = Context.Books.Where(x => x.ISBN == isbn);
            if (excludeId.HasValue)
            {
                int id = excludeId.Value;
                query = query.Where(x => x.Id != id);
            }
            return query.Any();
        }

        public bool HasRelatedData(int id)
        {
            return Context.Books.Any(x => x.Id == id &&
                (x.Authors.Any() || x.CartItems.Any() || x.Inventories.Any()
                 || x.OrderItems.Any() || x.PurchaseOrderItems.Any() || x.Reviews.Any()));
        }

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
