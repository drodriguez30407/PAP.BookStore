using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Generators
{
    public sealed class EmployeesGenerator
    {
        public GenerationResult Generate(int count)
        {
            return new GenerationService().Generate("Employees", count);
        }

        internal List<Employee> CreateBatch(int count, IList<Role> roles)
        {
            GeneratorTools.RequireParents(roles.Count, "Roles");
            var records = new List<Employee>();

            string key = GeneratorTools.Key();
            string hash = GeneratorTools.PasswordHash();
            DateTime now = DateTime.UtcNow;
            for (int i = 0; i < count; i++)
                records.Add(new Employee
                {
                    RoleId = roles[i % roles.Count].Id, FirstName = "Empleado " + (i + 1),
                    LastName = "Demostración " + key, Email = "empleado" + key + (i + 1) + "@example.com",
                    PasswordHash = hash, IsActive = true, CreatedAt = now
                });
            return records;
        }

        internal int GenerateRecords(BookStoreDBEntities context, int count)
        {
            var roles = context.Roles.AsNoTracking().OrderBy(x => x.Id).ToList();
            var batch = CreateBatch(count, roles);
            using (var repository = new EmployeeRepository(context))
            {
                repository.AddRange(batch);

            }
            return batch.Count;
        }
    }
}
