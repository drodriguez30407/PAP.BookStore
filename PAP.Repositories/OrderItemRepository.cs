using System;
using System.Collections.Generic;
using System.Linq;
using PAP.DataAccess;

namespace PAP.Repositories
{
    public class OrderItemRepository : EntityRepository<OrderItem>
    {
        public OrderItemRepository() : base(x => x.Id, CopyChanges) { }
        public OrderItemRepository(BookStoreDBEntities context) : base(context, x => x.Id, CopyChanges) { }

        private static void CopyChanges(OrderItem target, OrderItem source)
        {
            target.OrderId = source.OrderId;
            target.BookId = source.BookId;
            target.Quantity = source.Quantity;
            target.UnitPrice = source.UnitPrice;
            target.Discount = source.Discount;
        }

        public void RecalculateOrderTotals(IEnumerable<int> orderIds)
        {
            var ids = orderIds.Distinct().ToList();
            var totals = Context.OrderItems.Where(x => ids.Contains(x.OrderId))
                .GroupBy(x => x.OrderId).Select(g => new
                {
                    Id = g.Key, Subtotal = g.Sum(x => x.Quantity * x.UnitPrice - x.Discount)
                }).ToList().ToDictionary(x => x.Id, x => x.Subtotal);
            foreach (var order in Context.Orders.Where(x => ids.Contains(x.Id)).ToList())
            {
                order.Subtotal = totals[order.Id];
                order.Total = order.Subtotal + order.TaxAmount + order.ShippingCost;
                order.UpdatedAt = DateTime.UtcNow;
            }
            Context.SaveChanges();
        }

    }
}
