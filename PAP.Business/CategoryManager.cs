using System;
using System.Collections.Generic;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Business
{
    public class CategoryManager : IDisposable
    {
        private readonly CategoryRepository _repository = new CategoryRepository();

        public List<Category> GetAll()
        {
            return _repository.GetAll();
        }

        public Category GetById(int id)
        {
            ValidateId(id);

            var category = _repository.GetById(id);

            if (category == null)
            {
                throw new KeyNotFoundException("La categoría no existe.");
            }

            return category;
        }

        public void Add(Category category)
        {
            NormalizeAndValidate(category);

            if (_repository.NameExists(category.Name))
            {
                throw new InvalidOperationException(
                    "Ya existe una categoría con ese nombre.");
            }

            _repository.Add(category);
        }

        public void Update(Category category)
        {
            NormalizeAndValidate(category);
            GetById(category.Id);

            if (_repository.NameExists(category.Name, category.Id))
            {
                throw new InvalidOperationException(
                    "Ya existe otra categoría con ese nombre.");
            }

            _repository.Update(category);
        }

        public void Delete(int id)
        {
            GetById(id);

            if (_repository.HasBooks(id))
            {
                throw new InvalidOperationException(
                    "No se puede eliminar la categoría porque tiene libros asociados.");
            }

            _repository.Delete(id);
        }

        private static void NormalizeAndValidate(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                throw new ArgumentException("El nombre de la categoría es obligatorio.");
            }

            string name = category.Name.Trim();
            string description = string.IsNullOrWhiteSpace(category.Description)
                ? null
                : category.Description.Trim();

            if (name.Length > 100)
            {
                throw new ArgumentException(
                    "El nombre de la categoría no puede superar los 100 caracteres.");
            }

            if (description != null && description.Length > 500)
            {
                throw new ArgumentException(
                    "La descripción no puede superar los 500 caracteres.");
            }

            category.Name = name;
            category.Description = description;
        }

        private static void ValidateId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "El identificador de la categoría debe ser mayor que cero.");
            }
        }

        public void Dispose()
        {
            _repository.Dispose();
        }
    }
}
