using System;
using System.Collections.Generic;
using System.Linq;
using PAP.DataAccess;

namespace PAP.Repositories
{
    public class PaymentRepository : EntityRepository<Payment>
    {
        public PaymentRepository() : base(x => x.Id, CopyChanges) { }
        public PaymentRepository(BookStoreDBEntities context) : base(context, x => x.Id, CopyChanges) { }

        private static void CopyChanges(Payment target, Payment source)
        {
            target.OrderId = source.OrderId;
            target.PaymentMethod = source.PaymentMethod;
            target.Amount = source.Amount;
            target.PaymentDate = source.PaymentDate;
            target.Status = source.Status;
            target.TransactionRef = source.TransactionRef;
        }

        public void MarkOrdersPaid(IEnumerable<int> orderIds)
        {
            var ids = orderIds.Distinct().ToList();
            foreach (var order in Context.Orders.Where(x => ids.Contains(x.Id)).ToList())
            {
                order.Status = "Paid";
                order.UpdatedAt = DateTime.UtcNow;
            }
            Context.SaveChanges();
        }

    }
}
