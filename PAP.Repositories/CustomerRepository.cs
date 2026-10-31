using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PAP.DataAccess;

namespace PAP.Repositories
{
    public class CustomerRepository : EntityRepository<Customer>
    {
        public CustomerRepository() : base(x => x.Id, CopyChanges) { }
        public CustomerRepository(BookStoreDBEntities context) : base(context, x => x.Id, CopyChanges) { }

        public List<Customer> GetAllOrdered()
        {
            return Context.Customers.AsNoTracking().OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName).ThenBy(x => x.Id).ToList();
        }

        public bool EmailExists(string email, int? excludeId = null)
        {
            var query = Context.Customers.Where(x => x.Email == email);
            if (excludeId.HasValue)
            {
                int id = excludeId.Value;
                query = query.Where(x => x.Id != id);
            }
            return query.Any();
        }

        public bool HasRelatedData(int id)
        {
            return Context.Customers.Any(x => x.Id == id &&
                (x.Addresses.Any() || x.Orders.Any() || x.Reviews.Any() || x.ShoppingCarts.Any()));
        }

        private static void CopyChanges(Customer target, Customer source)
        {
            target.FirstName = source.FirstName;
            target.LastName = source.LastName;
            target.Email = source.Email;
            // An edit without a new password must preserve the existing hash.
            if (!string.IsNullOrEmpty(source.PasswordHash)) target.PasswordHash = source.PasswordHash;
            target.Phone = source.Phone;
            target.BirthDate = source.BirthDate;
            target.IsActive = source.IsActive;
            target.UpdatedAt = DateTime.UtcNow;
        }
    }
}
