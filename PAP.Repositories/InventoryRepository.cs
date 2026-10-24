using System;
using System.Collections.Generic;
using System.Linq;
using PAP.DataAccess;

namespace PAP.Repositories
{
    public class InventoryRepository : EntityRepository<Inventory>
    {
        public InventoryRepository() : base(x => x.Id, CopyChanges) { }
        public InventoryRepository(BookStoreDBEntities context) : base(context, x => x.Id, CopyChanges) { }

        private static void CopyChanges(Inventory target, Inventory source)
        {
            target.BookId = source.BookId;
            target.QuantityInStock = source.QuantityInStock;
            target.ReorderLevel = source.ReorderLevel;
            target.LastRestockedAt = source.LastRestockedAt;
        }

    }
}
