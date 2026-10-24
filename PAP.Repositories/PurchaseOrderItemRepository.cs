using System;
using System.Collections.Generic;
using System.Linq;
using PAP.DataAccess;

namespace PAP.Repositories
{
    public class PurchaseOrderItemRepository : EntityRepository<PurchaseOrderItem>
    {
        public PurchaseOrderItemRepository() : base(x => x.Id, CopyChanges) { }
        public PurchaseOrderItemRepository(BookStoreDBEntities context) : base(context, x => x.Id, CopyChanges) { }

        private static void CopyChanges(PurchaseOrderItem target, PurchaseOrderItem source)
        {
            target.PurchaseOrderId = source.PurchaseOrderId;
            target.BookId = source.BookId;
            target.Quantity = source.Quantity;
            target.UnitCost = source.UnitCost;
        }

        public void RecalculatePurchaseTotals(IEnumerable<int> purchaseOrderIds)
        {
            var ids = purchaseOrderIds.Distinct().ToList();
            var totals = Context.PurchaseOrderItems.Where(x => ids.Contains(x.PurchaseOrderId))
                .GroupBy(x => x.PurchaseOrderId).Select(g => new
                {
                    Id = g.Key, Total = g.Sum(x => x.Quantity * x.UnitCost)
                }).ToList().ToDictionary(x => x.Id, x => x.Total);
            foreach (var order in Context.PurchaseOrders.Where(x => ids.Contains(x.Id)).ToList())
                order.TotalCost = totals[order.Id];
            Context.SaveChanges();
        }

    }
}
