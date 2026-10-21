using System;
using System.Collections.Generic;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Business
{
    public class GenreManager : IDisposable
    {
        private readonly GenreRepository _repository = new GenreRepository();

        public List<Genre> GetAll()
        {
            return _repository.GetAll();
        }

        public Genre GetById(int id)
        {
            ValidateId(id);

            var genre = _repository.GetById(id);

            if (genre == null)
            {
                throw new KeyNotFoundException("El género no existe.");
            }

            return genre;
        }

        public void Add(Genre genre)
        {
            NormalizeAndValidate(genre);

            if (_repository.NameExists(genre.Name))
            {
                throw new InvalidOperationException(
                    "Ya existe un género con ese nombre.");
            }

            _repository.Add(genre);
        }

        public void Update(Genre genre)
        {
            NormalizeAndValidate(genre);
            GetById(genre.Id);

            if (_repository.NameExists(genre.Name, genre.Id))
            {
                throw new InvalidOperationException(
                    "Ya existe otro género con ese nombre.");
            }

            _repository.Update(genre);
        }

        public void Delete(int id)
        {
            GetById(id);

            if (_repository.HasBooks(id))
            {
                throw new InvalidOperationException(
                    "No se puede eliminar el género porque tiene libros asociados.");
            }

            _repository.Delete(id);
        }

        private static void NormalizeAndValidate(Genre genre)
        {
            if (genre == null)
            {
                throw new ArgumentNullException(nameof(genre));
            }

            if (string.IsNullOrWhiteSpace(genre.Name))
            {
                throw new ArgumentException("El nombre del género es obligatorio.");
            }

            string name = genre.Name.Trim();
            string description = string.IsNullOrWhiteSpace(genre.Description)
                ? null
                : genre.Description.Trim();

            if (name.Length > 100)
            {
                throw new ArgumentException(
                    "El nombre del género no puede superar los 100 caracteres.");
            }

            if (description != null && description.Length > 500)
            {
                throw new ArgumentException(
                    "La descripción no puede superar los 500 caracteres.");
            }

            genre.Name = name;
            genre.Description = description;
        }

        private static void ValidateId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "El identificador del género debe ser mayor que cero.");
            }
        }

        public void Dispose()
        {
            _repository.Dispose();
        }
    }
}
