using System;
using System.Collections.Generic;
using System.Linq;
using PAP.DataAccess;

namespace PAP.Repositories
{
    public sealed class BookAuthorRepository : IDisposable
    {
        private readonly BookStoreDBEntities _context;
        private readonly bool _ownsContext;

        public BookAuthorRepository() { _context = new BookStoreDBEntities(); _ownsContext = true; }
        public BookAuthorRepository(BookStoreDBEntities context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            _context = context;
        }

        public bool Any() { return _context.Books.Any(b => b.Authors.Any()); }

        public List<BookAuthorLink> GetAll()
        {
            return _context.Books.SelectMany(b => b.Authors.Select(a =>
                new BookAuthorLink { BookId = b.Id, AuthorId = a.Id })).ToList();
        }

        public void AddRange(IEnumerable<BookAuthorLink> links)
        {
            if (links == null) throw new ArgumentNullException(nameof(links));
            var batch = links.ToList();
            var bookIds = batch.Select(x => x.BookId).Distinct().ToList();
            var authorIds = batch.Select(x => x.AuthorId).Distinct().ToList();
            var books = _context.Books.Where(b => bookIds.Contains(b.Id)).ToDictionary(b => b.Id);
            var authors = _context.Authors.Where(a => authorIds.Contains(a.Id)).ToDictionary(a => a.Id);
            bool previous = _context.Configuration.LazyLoadingEnabled;
            try
            {
                _context.Configuration.LazyLoadingEnabled = false;
                foreach (var link in batch)
                {
                    Book book;
                    Author author;
                    if (!books.TryGetValue(link.BookId, out book) || !authors.TryGetValue(link.AuthorId, out author))
                        throw new InvalidOperationException("El libro o el autor ya no existe.");
                    book.Authors.Add(author);
                }
                _context.SaveChanges();
            }
            finally { _context.Configuration.LazyLoadingEnabled = previous; }
        }

        public void Add(BookAuthorLink link) { AddRange(new[] { link }); }

        public void Delete(int bookId, int authorId)
        {
            var book = _context.Books.Find(bookId);
            if (book == null) throw new KeyNotFoundException("El libro no existe.");
            _context.Entry(book).Collection(b => b.Authors).Load();
            var author = book.Authors.FirstOrDefault(a => a.Id == authorId);
            if (author == null) throw new KeyNotFoundException("La relación no existe.");
            book.Authors.Remove(author);
            _context.SaveChanges();
        }

        public void Dispose() { if (_ownsContext) _context.Dispose(); }
    }
}
