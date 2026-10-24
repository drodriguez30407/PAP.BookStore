using System;
using System.Collections.Generic;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Business
{
    public class AuthorManager : IDisposable
    {
        private readonly AuthorRepository _repository = new AuthorRepository();

        public List<Author> GetAll()
        {
            return _repository.GetAll();
        }

        public Author GetById(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "El identificador del autor debe ser mayor que cero!");
            }

            var author = _repository.GetById(id);

            if (author == null)
            {
                throw new KeyNotFoundException("El autor no existe!");
            }

            return author;
        }

        public void Add(Author author)
        {
            NormalizeAndValidate(author);
            author.CreatedAt = DateTime.UtcNow;
            _repository.Add(author);
        }

        public void Update(Author author)
        {
            NormalizeAndValidate(author);
            GetById(author.Id);
            _repository.Update(author);
        }

        public void Delete(int id)
        {
            GetById(id);

            if (_repository.HasBooks(id))
            {
                throw new InvalidOperationException(
                    "No se puede eliminar el autor porque tiene libros asociados.");
            }

            _repository.Delete(id);
        }

        private static void NormalizeAndValidate(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            string firstName = NormalizeRequiredName(author.FirstName, "Nombre del autor");
            string lastName = NormalizeRequiredName(author.LastName, "Apellido del autor");
            string biography = NormalizeOptional(author.Biography);
            string country = NormalizeOptional(author.Country);

            if (country != null && country.Length > 100)
            {
                throw new ArgumentException("El país no puede superar los 100 caracteres.");
            }

            DateTime? birthDate = author.BirthDate.HasValue
                ? author.BirthDate.Value.Date
                : (DateTime?)null;

            if (birthDate.HasValue && birthDate.Value > DateTime.Today)
            {
                throw new ArgumentException(
                    "La fecha de nacimiento no puede ser posterior a la fecha actual.");
            }

            author.FirstName = firstName;
            author.LastName = lastName;
            author.Biography = biography;
            author.BirthDate = birthDate;
            author.Country = country;
        }

        private static string NormalizeRequiredName(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(fieldName + " es obligatorio.");
            }

            string normalized = value.Trim();

            if (normalized.Length > 100)
            {
                throw new ArgumentException(
                    fieldName + " no puede superar los 100 caracteres.");
            }

            return normalized;
        }

        private static string NormalizeOptional(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        public void Dispose()
        {
            _repository.Dispose();
        }
    }
}
