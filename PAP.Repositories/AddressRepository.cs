using System;
using System.Collections.Generic;
using System.Linq;
using PAP.DataAccess;

namespace PAP.Repositories
{
    public class AddressRepository : EntityRepository<Address>
    {
        public AddressRepository() : base(x => x.Id, CopyChanges) { }
        public AddressRepository(BookStoreDBEntities context) : base(context, x => x.Id, CopyChanges) { }

        private static void CopyChanges(Address target, Address source)
        {
            target.CustomerId = source.CustomerId;
            target.AddressType = source.AddressType;
            target.Street = source.Street;
            target.City = source.City;
            target.State = source.State;
            target.PostalCode = source.PostalCode;
            target.Country = source.Country;
            target.IsDefault = source.IsDefault;
        }

    }
}
