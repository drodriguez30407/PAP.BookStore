using System;
using System.Collections.Generic;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Generators
{
    public sealed class AuthorsGenerator
    {
        public GenerationResult Generate(int count)
        {
            return new GenerationService().Generate("Authors", count);
        }

        internal List<Author> CreateBatch(int count)
        {
            string key = Guid.NewGuid().ToString("N").Substring(0, 8);
            var records = new List<Author>();
            DateTime createdAt = DateTime.UtcNow;
            DateTime referenceDate = DateTime.Today.AddYears(-30);
            for (int i = 1; i <= count; i++)
            {
                records.Add(new Author
                {
                    FirstName = "Autor " + i,
                    LastName = "Demostración " + key,
                    Biography = "Autor generado para pruebas.",
                    BirthDate = referenceDate.AddDays(-i),
                    Country = "Costa Rica",
                    CreatedAt = createdAt
                });
            }
            return records;
        }

        internal int GenerateRecords(BookStoreDBEntities context, int count)
        {
            using (var repository = new AuthorRepository(context))
            {
                repository.AddRange(CreateBatch(count));
            }
            return count;
        }
    }
}
