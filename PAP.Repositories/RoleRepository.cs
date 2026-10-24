using System;
using System.Collections.Generic;
using System.Linq;
using PAP.DataAccess;

namespace PAP.Repositories
{
    public class RoleRepository : EntityRepository<Role>
    {
        public RoleRepository() : base(x => x.Id, CopyChanges) { }
        public RoleRepository(BookStoreDBEntities context) : base(context, x => x.Id, CopyChanges) { }

        private static void CopyChanges(Role target, Role source)
        {
            target.Name = source.Name;
        }

    }
}
