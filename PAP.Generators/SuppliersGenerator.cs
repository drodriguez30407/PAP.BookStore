using System;
using System.Collections.Generic;
using System.Globalization;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Generators
{
    public sealed class SuppliersGenerator
    {
        public GenerationResult Generate(int count)
        {
            return new GenerationService().Generate("Suppliers", count);
        }

        internal List<Supplier> CreateBatch(int count)
        {
            string key = Guid.NewGuid().ToString("N").Substring(0, 8);
            var records = new List<Supplier>();
            for (int i = 1; i <= count; i++)
            {
                records.Add(new Supplier
                {
                    Name = "Proveedor " + i + " " + key,
                    ContactName = "Contacto de prueba " + i,
                    Email = "proveedor" + key + i + "@example.com",
                    Phone = "+506 2222-" + (1000 + i).ToString("0000", CultureInfo.InvariantCulture),
                    Address = "Dirección de demostración " + i + ", Costa Rica",
                    IsActive = true
                });
            }
            return records;
        }

        internal int GenerateRecords(BookStoreDBEntities context, int count)
        {
            using (var repository = new SupplierRepository(context))
            {
                repository.AddRange(CreateBatch(count));
            }
            return count;
        }
    }
}
