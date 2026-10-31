using System;
using System.Collections.Generic;
using System.Net.Mail;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Business
{
    public class CustomerManager : IDisposable
    {
        private readonly CustomerRepository _repository = new CustomerRepository();

        public List<Customer> GetAll() { return _repository.GetAllOrdered(); }

        public Customer GetById(int id)
        {
            if (id <= 0) throw new ArgumentException("El identificador del cliente debe ser mayor que cero.");
            var customer = _repository.GetById(id);
            if (customer == null) throw new KeyNotFoundException("El cliente no existe.");
            return customer;
        }

        public void Add(Customer customer, string password, string confirmPassword)
        {
            NormalizeAndValidate(customer);
            ValidatePassword(password, confirmPassword, true);
            if (_repository.EmailExists(customer.Email))
                throw new InvalidOperationException("Ya existe un cliente con ese correo electrónico.");
            customer.PasswordHash = PasswordHasher.Hash(password);
            DateTime now = DateTime.UtcNow;
            customer.CreatedAt = now;
            customer.UpdatedAt = now;
            _repository.Add(customer);
        }

        public void Update(Customer customer, string password, string confirmPassword)
        {
            NormalizeAndValidate(customer);
            ValidatePassword(password, confirmPassword, false);
            GetById(customer.Id);
            if (_repository.EmailExists(customer.Email, customer.Id))
                throw new InvalidOperationException("Ya existe otro cliente con ese correo electrónico.");
            customer.PasswordHash = string.IsNullOrEmpty(password) ? null : PasswordHasher.Hash(password);
            _repository.Update(customer);
        }

        public void Delete(int id)
        {
            GetById(id);
            if (_repository.HasRelatedData(id))
                throw new InvalidOperationException(
                    "No se puede eliminar el cliente porque tiene direcciones, pedidos, reseñas o carritos asociados.");
            _repository.Delete(id);
        }

        private static void NormalizeAndValidate(Customer customer)
        {
            if (customer == null) throw new ArgumentNullException(nameof(customer));
            string firstName = RequiredText(customer.FirstName, 100, "El nombre");
            string lastName = RequiredText(customer.LastName, 100, "El apellido");
            string email = RequiredText(customer.Email, 150, "El correo electrónico");
            string phone = string.IsNullOrWhiteSpace(customer.Phone) ? null : customer.Phone.Trim();
            if (phone != null && phone.Length > 30)
                throw new ArgumentException("El teléfono no puede superar los 30 caracteres.");
            try
            {
                var address = new MailAddress(email);
                if (!string.Equals(address.Address, email, StringComparison.OrdinalIgnoreCase))
                    throw new FormatException();
            }
            catch (FormatException)
            {
                throw new ArgumentException("Ingrese un correo electrónico válido.");
            }
            if (customer.BirthDate.HasValue && customer.BirthDate.Value.Date > DateTime.Today)
                throw new ArgumentException("La fecha de nacimiento no puede ser futura.");

            customer.FirstName = firstName;
            customer.LastName = lastName;
            customer.Email = email;
            customer.Phone = phone;
            if (customer.BirthDate.HasValue) customer.BirthDate = customer.BirthDate.Value.Date;
        }

        private static string RequiredText(string value, int maxLength, string label)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(label + " es obligatorio.");
            string normalized = value.Trim();
            if (normalized.Length > maxLength)
                throw new ArgumentException(label + " no puede superar los " + maxLength + " caracteres.");
            return normalized;
        }

        private static void ValidatePassword(string password, string confirmation, bool required)
        {
            if (!required && string.IsNullOrEmpty(password) && string.IsNullOrEmpty(confirmation)) return;
            if (string.IsNullOrWhiteSpace(password) || password.Length < PasswordHasher.MinLength
                || password.Length > PasswordHasher.MaxLength)
                throw new ArgumentException("La contraseña debe tener entre 15 y 128 caracteres.");
            if (!string.Equals(password, confirmation, StringComparison.Ordinal))
                throw new ArgumentException("La contraseña y su confirmación no coinciden.");
        }

        public void Dispose() { _repository.Dispose(); }
    }
}
