using System;
using System.Collections.Generic;
using System.Net.Mail;
using PAP.DataAccess;
using PAP.Repositories;

namespace PAP.Business
{
    public class SupplierManager : IDisposable
    {
        private readonly SupplierRepository _repository = new SupplierRepository();

        public List<Supplier> GetAll()
        {
            return _repository.GetAll();
        }

        public Supplier GetById(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "El identificador del proveedor debe ser mayor que cero.");
            }

            var supplier = _repository.GetById(id);

            if (supplier == null)
            {
                throw new KeyNotFoundException("El proveedor no existe.");
            }

            return supplier;
        }

        public void Add(Supplier supplier)
        {
            NormalizeAndValidate(supplier);

            if (_repository.NameExists(supplier.Name))
            {
                throw new InvalidOperationException(
                    "Ya existe un proveedor con ese nombre.");
            }

            _repository.Add(supplier);
        }

        public void Update(Supplier supplier)
        {
            NormalizeAndValidate(supplier);
            GetById(supplier.Id);

            if (_repository.NameExists(supplier.Name, supplier.Id))
            {
                throw new InvalidOperationException(
                    "Ya existe otro proveedor con ese nombre.");
            }

            _repository.Update(supplier);
        }

        public void Delete(int id)
        {
            GetById(id);

            if (_repository.HasPurchaseOrders(id))
            {
                throw new InvalidOperationException(
                    "No se puede eliminar el proveedor porque tiene órdenes de compra asociadas.");
            }

            _repository.Delete(id);
        }

        private static void NormalizeAndValidate(Supplier supplier)
        {
            if (supplier == null)
            {
                throw new ArgumentNullException(nameof(supplier));
            }

            if (string.IsNullOrWhiteSpace(supplier.Name))
            {
                throw new ArgumentException("El nombre del proveedor es obligatorio.");
            }

            string name = supplier.Name.Trim();

            if (name.Length > 200)
            {
                throw new ArgumentException(
                    "El nombre del proveedor no puede superar los 200 caracteres.");
            }

            string contactName = NormalizeOptional(supplier.ContactName, 150, "El nombre del contacto");
            string email = NormalizeOptional(supplier.Email, 150, "El correo electrónico");
            string phone = NormalizeOptional(supplier.Phone, 30, "El teléfono");
            string address = NormalizeOptional(supplier.Address, 300, "La dirección");

            if (email != null && !IsValidEmail(email))
            {
                throw new ArgumentException("Ingrese un correo electrónico válido.");
            }

            supplier.Name = name;
            supplier.ContactName = contactName;
            supplier.Email = email;
            supplier.Phone = phone;
            supplier.Address = address;
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
