using System;
using System.Collections.Generic;
using System.Net.Mail;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Business
{
    public class PublisherManager : IDisposable
    {
        private readonly PublisherRepository _repository = new PublisherRepository();

        public List<Publisher> GetAll()
        {
            return _repository.GetAll();
        }

        public Publisher GetById(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "El identificador de la editorial debe ser mayor que cero.");
            }

            var publisher = _repository.GetById(id);

            if (publisher == null)
            {
                throw new KeyNotFoundException("La editorial no existe.");
            }

            return publisher;
        }

        public void Add(Publisher publisher)
        {
            NormalizeAndValidate(publisher);

            if (_repository.NameExists(publisher.Name))
            {
                throw new InvalidOperationException(
                    "Ya existe una editorial con ese nombre.");
            }

            publisher.CreatedAt = DateTime.UtcNow;
            _repository.Add(publisher);
        }

        public void Update(Publisher publisher)
        {
            NormalizeAndValidate(publisher);
            GetById(publisher.Id);

            if (_repository.NameExists(publisher.Name, publisher.Id))
            {
                throw new InvalidOperationException(
                    "Ya existe otra editorial con ese nombre.");
            }

            _repository.Update(publisher);
        }

        public void Delete(int id)
        {
            GetById(id);

            if (_repository.HasBooks(id))
            {
                throw new InvalidOperationException(
                    "No se puede eliminar la editorial porque tiene libros asociados.");
            }

            _repository.Delete(id);
        }

        private static void NormalizeAndValidate(Publisher publisher)
        {
            if (publisher == null)
            {
                throw new ArgumentNullException(nameof(publisher));
            }

            if (string.IsNullOrWhiteSpace(publisher.Name))
            {
                throw new ArgumentException("El nombre de la editorial es obligatorio.");
            }

            string name = publisher.Name.Trim();

            if (name.Length > 200)
            {
                throw new ArgumentException(
                    "El nombre de la editorial no puede superar los 200 caracteres.");
            }

            string country = NormalizeOptional(publisher.Country, 100, "El país");
            string website = NormalizeOptional(publisher.Website, 255, "El sitio web");
            string email = NormalizeOptional(publisher.Email, 150, "El correo electrónico");

            if (website != null)
            {
                Uri websiteUri;

                if (!Uri.TryCreate(website, UriKind.Absolute, out websiteUri)
                    || (websiteUri.Scheme != Uri.UriSchemeHttp
                        && websiteUri.Scheme != Uri.UriSchemeHttps)
                    || string.IsNullOrWhiteSpace(websiteUri.Host))
                {
                    throw new ArgumentException(
                        "Ingrese un sitio web válido que comience con http:// o https://.");
                }
            }

            if (email != null && !IsValidEmail(email))
            {
                throw new ArgumentException("Ingrese un correo electrónico válido.");
            }

            publisher.Name = name;
            publisher.Country = country;
            publisher.Website = website;
            publisher.Email = email;
        }

        private static string NormalizeOptional(string value, int maxLength, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            string normalized = value.Trim();

            if (normalized.Length > maxLength)
            {
                throw new ArgumentException(
                    fieldName + " no puede superar los " + maxLength + " caracteres.");
            }

            return normalized;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var address = new MailAddress(email);
                return string.Equals(address.Address, email, StringComparison.OrdinalIgnoreCase);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public void Dispose()
        {
            _repository.Dispose();
        }
    }
}
