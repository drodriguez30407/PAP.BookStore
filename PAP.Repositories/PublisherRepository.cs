using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PAP.DataAccess;

namespace PAP.Repositories
{
    public class PublisherRepository : IDisposable
    {
        private readonly BookStoreDBEntities _context;
        private readonly bool _ownsContext;

        public PublisherRepository()
        {
            _context = new BookStoreDBEntities();
            _ownsContext = true;
        }

        public PublisherRepository(BookStoreDBEntities context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;
            _ownsContext = false;
        }

        public List<Publisher> GetAll()
        {
            return _context.Publishers
                .AsNoTracking()
                .OrderBy(publisher => publisher.Name)
                .ThenBy(publisher => publisher.Id)
                .ToList();
        }

        public Publisher GetById(int id)
        {
            return _context.Publishers.Find(id);
        }

        public bool Any()
        {
            return _context.Publishers.Any();
        }

        public bool NameExists(string name, int? excludeId = null)
        {
            var query = _context.Publishers.Where(publisher => publisher.Name == name);

            if (excludeId.HasValue)
            {
                int id = excludeId.Value;
                query = query.Where(publisher => publisher.Id != id);
            }

            return query.Any();
        }

        public bool HasBooks(int publisherId)
        {
            return _context.Books.Any(book => book.PublisherId == publisherId);
        }

        public void Add(Publisher publisher)
        {
            if (publisher == null)
            {
                throw new ArgumentNullException(nameof(publisher));
            }

            _context.Publishers.Add(publisher);
            _context.SaveChanges();
        }

        public void AddRange(IEnumerable<Publisher> publishers)
        {
            if (publishers == null)
            {
                throw new ArgumentNullException(nameof(publishers));
            }

            _context.Publishers.AddRange(publishers);
            _context.SaveChanges();
        }

        public void Update(Publisher publisher)
        {
            if (publisher == null)
            {
                throw new ArgumentNullException(nameof(publisher));
            }

            var existingPublisher = _context.Publishers.Find(publisher.Id);

            if (existingPublisher == null)
            {
                throw new KeyNotFoundException("La editorial no existe.");
            }

            existingPublisher.Name = publisher.Name;
            existingPublisher.Country = publisher.Country;
            existingPublisher.Website = publisher.Website;
            existingPublisher.Email = publisher.Email;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var publisher = _context.Publishers.Find(id);

            if (publisher == null)
            {
                throw new KeyNotFoundException("La editorial no existe.");
            }

            _context.Publishers.Remove(publisher);
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
