using System;
using System.Collections.Generic;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Generators
{
    public sealed class CategoriesGenerator
    {
        public GenerationResult Generate(int count)
        {
            return new GenerationService().Generate("Categories", count);
        }

        internal List<Category> CreateBatch(int count)
        {
            string key = Guid.NewGuid().ToString("N").Substring(0, 8);
            var records = new List<Category>();
            for (int i = 1; i <= count; i++)
            {
                records.Add(new Category
                {
                    Name = "Categoría " + i + " " + key,
                    Description = "Categoría de libros generada para pruebas."
                });
            }
            return records;
        }

        internal int GenerateRecords(BookStoreDBEntities context, int count)
        {
            using (var repository = new CategoryRepository(context))
            {
                repository.AddRange(CreateBatch(count));
            }
            return count;
        }
    }
}
