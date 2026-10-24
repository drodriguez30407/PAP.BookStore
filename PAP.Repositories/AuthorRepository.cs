using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PAP.DataAccess;

namespace PAP.Repositories
{
    public class AuthorRepository : IDisposable
    {
        private readonly BookStoreDBEntities _context;
        private readonly bool _ownsContext;

        public AuthorRepository()
        {
            _context = new BookStoreDBEntities();
            _ownsContext = true;
        }

        public AuthorRepository(BookStoreDBEntities context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;
            _ownsContext = false;
        }

        public List<Author> GetAll()
        {
            return _context.Authors
                .AsNoTracking()
                .OrderBy(author => author.LastName)
                .ThenBy(author => author.FirstName)
                .ThenBy(author => author.Id)
                .ToList();
        }

        public Author GetById(int id)
        {
            return _context.Authors.Find(id);
        }

        public bool Any()
        {
            return _context.Authors.Any();
        }

        public bool HasBooks(int authorId)
        {
            return _context.Authors.Any(author => author.Id == authorId && author.Books.Any());
        }

        public void Add(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            _context.Authors.Add(author);
            _context.SaveChanges();
        }

        public void AddRange(IEnumerable<Author> authors)
        {
            if (authors == null)
            {
                throw new ArgumentNullException(nameof(authors));
            }

            _context.Authors.AddRange(authors);
            _context.SaveChanges();
        }

        public void Update(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            var existingAuthor = _context.Authors.Find(author.Id);

            if (existingAuthor == null)
            {
                throw new KeyNotFoundException("El autor no existe!");
            }

            existingAuthor.FirstName = author.FirstName;
            existingAuthor.LastName = author.LastName;
            existingAuthor.Biography = author.Biography;
            existingAuthor.BirthDate = author.BirthDate;
            existingAuthor.Country = author.Country;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var author = _context.Authors.Find(id);

            if (author == null)
            {
                throw new KeyNotFoundException("El autor no existe!");
            }

            _context.Authors.Remove(author);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            if (_ownsContext)
            {
                _context.Dispose();
            }
        }
    }
}
