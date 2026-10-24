using System;
using System.Collections.Generic;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Generators
{
    public sealed class PublishersGenerator
    {
        public GenerationResult Generate(int count)
        {
            return new GenerationService().Generate("Publishers", count);
        }

        internal List<Publisher> CreateBatch(int count)
        {
            string key = Guid.NewGuid().ToString("N").Substring(0, 8);
            var records = new List<Publisher>();
            DateTime createdAt = DateTime.UtcNow;
            for (int i = 1; i <= count; i++)
            {
                records.Add(new Publisher
                {
                    Name = "Editorial " + i + " " + key,
                    Country = "Costa Rica",
                    Website = "https://example.com/editoriales/" + key + "/" + i,
                    Email = "editorial" + key + i + "@example.com",
                    CreatedAt = createdAt
                });
            }
            return records;
        }

        internal int GenerateRecords(BookStoreDBEntities context, int count)
        {
            using (var repository = new PublisherRepository(context))
            {
                repository.AddRange(CreateBatch(count));
            }
            return count;
        }
    }
}
