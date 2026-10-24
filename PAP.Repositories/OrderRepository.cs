using System;
using System.Collections.Generic;
using System.Linq;
using PAP.DataAccess;

namespace PAP.Repositories
{
    public class OrderRepository : EntityRepository<Order>
    {
        public OrderRepository() : base(x => x.Id, CopyChanges) { }
        public OrderRepository(BookStoreDBEntities context) : base(context, x => x.Id, CopyChanges) { }

        private static void CopyChanges(Order target, Order source)
        {
            target.CustomerId = source.CustomerId;
            target.ShippingAddressId = source.ShippingAddressId;
            target.OrderDate = source.OrderDate;
            target.Status = source.Status;
            target.Subtotal = source.Subtotal;
            target.TaxAmount = source.TaxAmount;
            target.ShippingCost = source.ShippingCost;
            target.Total = source.Total;
            target.Notes = source.Notes;
            target.UpdatedAt = DateTime.UtcNow;
        }

    }
}
