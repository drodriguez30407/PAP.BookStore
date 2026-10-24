using System;
using System.Collections.Generic;
using System.Linq;
using PAP.DataAccess;

namespace PAP.Repositories
{
    public class ReviewRepository : EntityRepository<Review>
    {
        public ReviewRepository() : base(x => x.Id, CopyChanges) { }
        public ReviewRepository(BookStoreDBEntities context) : base(context, x => x.Id, CopyChanges) { }

        private static void CopyChanges(Review target, Review source)
        {
            target.BookId = source.BookId;
            target.CustomerId = source.CustomerId;
            target.Rating = source.Rating;
            target.Comment = source.Comment;
        }

    }
}
