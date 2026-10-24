using System;
using System.Collections.Generic;
using System.Linq;
using PAP.DataAccess;

namespace PAP.Repositories
{
    public class ShoppingCartRepository : EntityRepository<ShoppingCart>
    {
        public ShoppingCartRepository() : base(x => x.Id, CopyChanges) { }
        public ShoppingCartRepository(BookStoreDBEntities context) : base(context, x => x.Id, CopyChanges) { }

        private static void CopyChanges(ShoppingCart target, ShoppingCart source)
        {
            target.CustomerId = source.CustomerId;
            target.UpdatedAt = DateTime.UtcNow;
        }

    }
}
