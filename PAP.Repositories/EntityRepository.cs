using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PAP.DataAccess;

namespace PAP.Repositories
{
    public abstract class EntityRepository<T> : IDisposable where T : class
    {
        protected readonly BookStoreDBEntities Context;
        private readonly bool _ownsContext;
        private readonly Func<T, int> _getId;
        private readonly Action<T, T> _copyChanges;

        protected EntityRepository(Func<T, int> getId, Action<T, T> copyChanges)
            : this(new BookStoreDBEntities(), true, getId, copyChanges) { }

        protected EntityRepository(BookStoreDBEntities context, Func<T, int> getId, Action<T, T> copyChanges)
            : this(context, false, getId, copyChanges) { }

        private EntityRepository(BookStoreDBEntities context, bool ownsContext,
            Func<T, int> getId, Action<T, T> copyChanges)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Context = context;
            _ownsContext = ownsContext;
            _getId = getId;
            _copyChanges = copyChanges;
        }

        public List<T> GetAll() { return Context.Set<T>().AsNoTracking().ToList(); }
        public T GetById(int id) { return Context.Set<T>().Find(id); }
        public bool Any() { return Context.Set<T>().Any(); }

        public void Add(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            Context.Set<T>().Add(entity);
            Context.SaveChanges();
        }

        public void AddRange(IEnumerable<T> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            Context.Set<T>().AddRange(entities);
            Context.SaveChanges();
        }

        public void Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var existing = GetById(_getId(entity));
            if (existing == null) throw new KeyNotFoundException("El registro no existe.");
            _copyChanges(existing, entity);
            Context.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            if (entity == null) throw new KeyNotFoundException("El registro no existe.");
            Context.Set<T>().Remove(entity);
            Context.SaveChanges();
        }

        public void Dispose() { if (_ownsContext) Context.Dispose(); }
    }
}
