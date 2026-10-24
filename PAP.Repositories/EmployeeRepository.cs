using System;
using System.Collections.Generic;
using System.Linq;
using PAP.DataAccess;

namespace PAP.Repositories
{
    public class EmployeeRepository : EntityRepository<Employee>
    {
        public EmployeeRepository() : base(x => x.Id, CopyChanges) { }
        public EmployeeRepository(BookStoreDBEntities context) : base(context, x => x.Id, CopyChanges) { }

        private static void CopyChanges(Employee target, Employee source)
        {
            target.RoleId = source.RoleId;
            target.FirstName = source.FirstName;
            target.LastName = source.LastName;
            target.Email = source.Email;
            target.PasswordHash = source.PasswordHash;
            target.IsActive = source.IsActive;
        }

    }
}
