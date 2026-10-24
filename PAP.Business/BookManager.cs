using System;
using System.Collections.Generic;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Business
{
    public class BookManager : IDisposable
    {
        private readonly BookRepository _repository = new BookRepository();

        public List<Book> GetAll() { return _repository.GetAllWithDetails(); }
        public List<Category> GetCategories() { return _repository.GetCategories(); }
        public List<Genre> GetGenres() { return _repository.GetGenres(); }
        public List<Publisher> GetPublishers() { return _repository.GetPublishers(); }

        public Book GetById(int id)
        {
            if (id <= 0) throw new ArgumentException("El identificador del libro debe ser mayor que cero.");
            var book = _repository.GetWithDetails(id);
            if (book == null) throw new KeyNotFoundException("El libro no existe.");
            return book;
        }

        public void Add(Book book)
        {
            NormalizeAndValidate(book);
            ValidateReferences(book);
            if (book.ISBN != null && _repository.IsbnExists(book.ISBN))
                throw new InvalidOperationException("Ya existe un libro con ese ISBN.");
            DateTime now = DateTime.UtcNow;
            book.CreatedAt = now;
            book.UpdatedAt = now;
            _repository.Add(book);
        }

        public void Update(Book book)
        {
            NormalizeAndValidate(book);
            GetById(book.Id);
            ValidateReferences(book);
            if (book.ISBN != null && _repository.IsbnExists(book.ISBN, book.Id))
                throw new InvalidOperationException("Ya existe otro libro con ese ISBN.");
            _repository.Update(book);
        }

        public void Delete(int id)
        {
            GetById(id);
            if (_repository.HasRelatedData(id))
                throw new InvalidOperationException(
                    "No se puede eliminar el libro porque tiene autores, inventario, pedidos, compras, carritos o reseñas asociados.");
            _repository.Delete(id);
        }

        private void ValidateReferences(Book book)
        {
            if (book.CategoryId.HasValue && !_repository.CategoryExists(book.CategoryId.Value))
                throw new ArgumentException("La categoría seleccionada no existe. Actualice la página.");
            if (book.GenreId.HasValue && !_repository.GenreExists(book.GenreId.Value))
                throw new ArgumentException("El género seleccionado no existe. Actualice la página.");
            if (book.PublisherId.HasValue && !_repository.PublisherExists(book.PublisherId.Value))
                throw new ArgumentException("La editorial seleccionada no existe. Actualice la página.");
        }

        private static void NormalizeAndValidate(Book book)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));
            if (string.IsNullOrWhiteSpace(book.Title))
                throw new ArgumentException("El título del libro es obligatorio.");
            string title = book.Title.Trim();
            if (title.Length > 200)
                throw new ArgumentException("El título no puede superar los 200 caracteres.");
            if (book.Price < 0 || book.Price > 9999999999999999.99m)
                throw new ArgumentException("El precio debe estar entre 0 y 9999999999999999.99.");
            if (decimal.Round(book.Price, 2) != book.Price)
                throw new ArgumentException("El precio puede tener como máximo dos decimales.");
            if (book.Pages.HasValue && book.Pages.Value <= 0)
                throw new ArgumentException("La cantidad de páginas debe ser mayor que cero.");
            if ((book.CategoryId.HasValue && book.CategoryId.Value <= 0)
                || (book.GenreId.HasValue && book.GenreId.Value <= 0)
                || (book.PublisherId.HasValue && book.PublisherId.Value <= 0))
                throw new ArgumentException("Seleccione una categoría, género o editorial válida, o deje la opción sin asignar.");

            string isbn = NormalizeOptional(book.ISBN, 20, "El ISBN");
            string language = NormalizeOptional(book.Language, 50, "El idioma");
            string coverUrl = NormalizeOptional(book.CoverImageUrl, 500, "La URL de la portada");
            if (coverUrl != null)
            {
                Uri uri;
                if (!Uri.TryCreate(coverUrl, UriKind.Absolute, out uri)
                    || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                    || string.IsNullOrWhiteSpace(uri.Host))
                    throw new ArgumentException("La URL de la portada debe comenzar con http:// o https:// y tener un servidor válido.");
            }

            book.Title = title;
            book.Description = string.IsNullOrWhiteSpace(book.Description) ? null : book.Description.Trim();
            book.ISBN = isbn;
            book.Language = language;
            book.CoverImageUrl = coverUrl;
            if (book.PublicationDate.HasValue) book.PublicationDate = book.PublicationDate.Value.Date;
        }

        private static string NormalizeOptional(string value, int maxLength, string label)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            string normalized = value.Trim();
            if (normalized.Length > maxLength)
                throw new ArgumentException(label + " no puede superar los " + maxLength + " caracteres.");
            return normalized;
        }

        public void Dispose() { _repository.Dispose(); }
    }
}
