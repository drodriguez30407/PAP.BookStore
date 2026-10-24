using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Generators
{
    public sealed class RolesGenerator
    {
        public GenerationResult Generate(int count)
        {
            return new GenerationService().Generate("Roles", count);
        }

        internal List<Role> CreateBatch(int count)
        {

            var records = new List<Role>();

            string key = GeneratorTools.Key();
            string[] names = { "Admin", "Manager", "Cashier" };
            for (int i = 0; i < count; i++)
                records.Add(new Role { Name = i < names.Length ? names[i] : "Rol " + (i + 1) + " " + key });
            return records;
        }

        internal int GenerateRecords(BookStoreDBEntities context, int count)
        {

            var batch = CreateBatch(count);
            using (var repository = new RoleRepository(context))
            {
                repository.AddRange(batch);

            }
            return batch.Count;
        }
    }
}
