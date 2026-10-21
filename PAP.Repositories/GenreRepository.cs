using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PAP.DataAccess;

namespace PAP.Repositories
{
    public class GenreRepository : IDisposable
    {
        private readonly BookStoreDBEntities _context;
        private readonly bool _ownsContext;

        public GenreRepository()
        {
            _context = new BookStoreDBEntities();
            _ownsContext = true;
        }

        public GenreRepository(BookStoreDBEntities context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;
            _ownsContext = false;
        }

        public List<Genre> GetAll()
        {
            return _context.Genres
                .AsNoTracking()
                .OrderBy(genre => genre.Name)
                .ToList();
        }

        public Genre GetById(int id)
        {
            return _context.Genres.Find(id);
        }

        public bool Any()
        {
            return _context.Genres.Any();
        }

        public bool NameExists(string name, int? excludeId = null)
        {
            var query = _context.Genres.Where(genre => genre.Name == name);

            if (excludeId.HasValue)
            {
                int id = excludeId.Value;
                query = query.Where(genre => genre.Id != id);
            }

            return query.Any();
        }

        public bool HasBooks(int genreId)
        {
            return _context.Books.Any(book => book.GenreId == genreId);
        }

        public void Add(Genre genre)
        {
            if (genre == null)
            {
                throw new ArgumentNullException(nameof(genre));
            }

            _context.Genres.Add(genre);
            _context.SaveChanges();
        }

        public void AddRange(IEnumerable<Genre> genres)
        {
            if (genres == null)
            {
                throw new ArgumentNullException(nameof(genres));
            }

            _context.Genres.AddRange(genres);
            _context.SaveChanges();
        }

        public void Update(Genre genre)
        {
            if (genre == null)
            {
                throw new ArgumentNullException(nameof(genre));
            }

            var existingGenre = _context.Genres.Find(genre.Id);

            if (existingGenre == null)
            {
                throw new KeyNotFoundException("El género no existe.");
            }

            existingGenre.Name = genre.Name;
            existingGenre.Description = genre.Description;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var genre = _context.Genres.Find(id);

            if (genre == null)
            {
                throw new KeyNotFoundException("El género no existe.");
            }

            _context.Genres.Remove(genre);
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
