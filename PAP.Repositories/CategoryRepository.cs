using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PAP.DataAccess;

namespace PAP.Repositories
{
    public class CategoryRepository : IDisposable
    {
        private readonly BookStoreDBEntities _context;
        private readonly bool _ownsContext;

        public CategoryRepository()
        {
            _context = new BookStoreDBEntities();
            _ownsContext = true;
        }

        public CategoryRepository(BookStoreDBEntities context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;
            _ownsContext = false;
        }

        public List<Category> GetAll()
        {
            return _context.Categories
                .AsNoTracking()
                .OrderBy(category => category.Name)
                .ToList();
        }

        public Category GetById(int id)
        {
            return _context.Categories.Find(id);
        }

        public bool Any()
        {
            return _context.Categories.Any();
        }

        public bool NameExists(string name, int? excludeId = null)
        {
            var query = _context.Categories.Where(category => category.Name == name);

            if (excludeId.HasValue)
            {
                int id = excludeId.Value;
                query = query.Where(category => category.Id != id);
            }

            return query.Any();
        }

        public bool HasBooks(int categoryId)
        {
            return _context.Books.Any(book => book.CategoryId == categoryId);
        }

        public void Add(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public void AddRange(IEnumerable<Category> categories)
        {
            if (categories == null)
            {
                throw new ArgumentNullException(nameof(categories));
            }

            _context.Categories.AddRange(categories);
            _context.SaveChanges();
        }

        public void Update(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            var existingCategory = _context.Categories.Find(category.Id);

            if (existingCategory == null)
            {
                throw new KeyNotFoundException("La categoria no existe.");
            }

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var category = _context.Categories.Find(id);

            if (category == null)
            {
                throw new KeyNotFoundException("La categoria no existe.");
            }

            _context.Categories.Remove(category);
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
