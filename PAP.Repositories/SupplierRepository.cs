using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PAP.DataAccess;

namespace PAP.Repositories
{
    public class SupplierRepository : IDisposable
    {
        private readonly BookStoreDBEntities _context;
        private readonly bool _ownsContext;

        public SupplierRepository()
        {
            _context = new BookStoreDBEntities();
            _ownsContext = true;
        }

        public SupplierRepository(BookStoreDBEntities context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;
            _ownsContext = false;
        }

        public List<Supplier> GetAll()
        {
            return _context.Suppliers
                .AsNoTracking()
                .OrderBy(supplier => supplier.Name)
                .ThenBy(supplier => supplier.Id)
                .ToList();
        }

        public Supplier GetById(int id)
        {
            return _context.Suppliers.Find(id);
        }

        public bool Any()
        {
            return _context.Suppliers.Any();
        }

        public bool NameExists(string name, int? excludeId = null)
        {
            var query = _context.Suppliers.Where(supplier => supplier.Name == name);

            if (excludeId.HasValue)
            {
                int id = excludeId.Value;
                query = query.Where(supplier => supplier.Id != id);
            }

            return query.Any();
        }

        public bool HasPurchaseOrders(int supplierId)
        {
            return _context.PurchaseOrders.Any(order => order.SupplierId == supplierId);
        }

        public void Add(Supplier supplier)
        {
            if (supplier == null)
            {
                throw new ArgumentNullException(nameof(supplier));
            }

            _context.Suppliers.Add(supplier);
            _context.SaveChanges();
        }

        public void AddRange(IEnumerable<Supplier> suppliers)
        {
            if (suppliers == null)
            {
                throw new ArgumentNullException(nameof(suppliers));
            }

            _context.Suppliers.AddRange(suppliers);
            _context.SaveChanges();
        }

        public void Update(Supplier supplier)
        {
            if (supplier == null)
            {
                throw new ArgumentNullException(nameof(supplier));
            }

            var existingSupplier = _context.Suppliers.Find(supplier.Id);

            if (existingSupplier == null)
            {
                throw new KeyNotFoundException("El proveedor no existe.");
            }

            existingSupplier.Name = supplier.Name;
            existingSupplier.ContactName = supplier.ContactName;
            existingSupplier.Email = supplier.Email;
            existingSupplier.Phone = supplier.Phone;
            existingSupplier.Address = supplier.Address;
            existingSupplier.IsActive = supplier.IsActive;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var supplier = _context.Suppliers.Find(id);

            if (supplier == null)
            {
                throw new KeyNotFoundException("El proveedor no existe.");
            }

            _context.Suppliers.Remove(supplier);
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
