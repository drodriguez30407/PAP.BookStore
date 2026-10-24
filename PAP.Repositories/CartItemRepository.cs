using System;
using System.Collections.Generic;
using System.Linq;
using PAP.DataAccess;

namespace PAP.Repositories
{
    public class CartItemRepository : EntityRepository<CartItem>
    {
        public CartItemRepository() : base(x => x.Id, CopyChanges) { }
        public CartItemRepository(BookStoreDBEntities context) : base(context, x => x.Id, CopyChanges) { }

        private static void CopyChanges(CartItem target, CartItem source)
        {
            target.CartId = source.CartId;
            target.BookId = source.BookId;
            target.Quantity = source.Quantity;
            target.AddedAt = source.AddedAt;
        }

    }
}
