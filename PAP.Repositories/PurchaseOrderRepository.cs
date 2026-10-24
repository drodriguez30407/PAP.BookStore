using System;
using System.Collections.Generic;
using System.Linq;
using PAP.DataAccess;

namespace PAP.Repositories
{
    public class PurchaseOrderRepository : EntityRepository<PurchaseOrder>
    {
        public PurchaseOrderRepository() : base(x => x.Id, CopyChanges) { }
        public PurchaseOrderRepository(BookStoreDBEntities context) : base(context, x => x.Id, CopyChanges) { }

        private static void CopyChanges(PurchaseOrder target, PurchaseOrder source)
        {
            target.SupplierId = source.SupplierId;
            target.OrderDate = source.OrderDate;
            target.ExpectedDate = source.ExpectedDate;
            target.Status = source.Status;
            target.TotalCost = source.TotalCost;
        }

    }
}
