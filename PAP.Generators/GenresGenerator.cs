using System;
using System.Collections.Generic;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Generators
{
    public sealed class GenresGenerator
    {
        public GenerationResult Generate(int count)
        {
            return new GenerationService().Generate("Genres", count);
        }

        internal List<Genre> CreateBatch(int count)
        {
            string key = Guid.NewGuid().ToString("N").Substring(0, 8);
            var records = new List<Genre>();
            for (int i = 1; i <= count; i++)
            {
                records.Add(new Genre
                {
                    Name = "Género " + i + " " + key,
                    Description = "Género literario generado para pruebas."
                });
            }
            return records;
        }

        internal int GenerateRecords(BookStoreDBEntities context, int count)
        {
            using (var repository = new GenreRepository(context))
            {
                repository.AddRange(CreateBatch(count));
            }
            return count;
        }
    }
}
