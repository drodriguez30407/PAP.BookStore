using System;
using System.Collections.Generic;
using System.Linq;
using PAP.DataAccess;

namespace PAP.Repositories
{
    public class CustomerRepository : EntityRepository<Customer>
    {
        public CustomerRepository() : base(x => x.Id, CopyChanges) { }
        public CustomerRepository(BookStoreDBEntities context) : base(context, x => x.Id, CopyChanges) { }

        private static void CopyChanges(Customer target, Customer source)
        {
            target.FirstName = source.FirstName;
            target.LastName = source.LastName;
            target.Email = source.Email;
            target.PasswordHash = source.PasswordHash;
            target.Phone = source.Phone;
            target.BirthDate = source.BirthDate;
            target.IsActive = source.IsActive;
            target.UpdatedAt = DateTime.UtcNow;
        }

    }
}
