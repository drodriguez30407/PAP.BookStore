using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using PAP.DataAccess;

namespace PAP.Generators
{
    public sealed class GenerationService
    {
        public const int MaxBatchSize = 500;

        private sealed class EntityDefinition
        {
            internal readonly string Name;
            internal readonly string DisplayName;
            internal readonly Func<BookStoreDBEntities, bool> HasData;
            internal readonly string[] Dependencies;
            internal readonly Func<BookStoreDBEntities, int, int> Write;

            internal EntityDefinition(string name, string displayName,
                Func<BookStoreDBEntities, bool> hasData, string[] dependencies,
                Func<BookStoreDBEntities, int, int> write = null)
            {
                Name = name;
                DisplayName = displayName;
                HasData = hasData;
                Dependencies = dependencies;
                Write = write;
            }
        }

        // Las dependencias describen el orden en que se generaran registros
        // con sus relaciones. Tambien se comprueban al recibir cada POST.
        private static readonly EntityDefinition[] Definitions =
        {
            new EntityDefinition("Addresses", "Direcciones", c => c.Addresses.Any(), new[] { "Customers" }),
            new EntityDefinition("Authors", "Autores", c => c.Authors.Any(), new string[0],
                new AuthorsGenerator().GenerateRecords),
            new EntityDefinition("BookAuthors", "Libros y autores", c => c.Books.Any(b => b.Authors.Any()),
                new[] { "Books", "Authors" }),
            new EntityDefinition("Books", "Libros", c => c.Books.Any(),
                new[] { "Categories", "Genres", "Publishers" }),
            new EntityDefinition("CartItems", "Artículos del carrito", c => c.CartItems.Any(),
                new[] { "ShoppingCarts", "Books" }),
            new EntityDefinition("Categories", "Categorías", c => c.Categories.Any(), new string[0],
                new CategoriesGenerator().GenerateRecords),
            new EntityDefinition("Customers", "Clientes", c => c.Customers.Any(), new string[0]),
            new EntityDefinition("Employees", "Empleados", c => c.Employees.Any(), new[] { "Roles" }),
            new EntityDefinition("Genres", "Géneros", c => c.Genres.Any(), new string[0],
                new GenresGenerator().GenerateRecords),
            new EntityDefinition("Inventory", "Inventario", c => c.Inventories.Any(), new[] { "Books" }),
            new EntityDefinition("OrderItems", "Detalles de pedidos", c => c.OrderItems.Any(),
                new[] { "Orders", "Books" }),
            new EntityDefinition("Orders", "Pedidos", c => c.Orders.Any(), new[] { "Customers", "Addresses" }),
            new EntityDefinition("Payments", "Pagos", c => c.Payments.Any(), new[] { "Orders" }),
            new EntityDefinition("Publishers", "Editoriales", c => c.Publishers.Any(), new string[0],
                new PublishersGenerator().GenerateRecords),
            new EntityDefinition("PurchaseOrderItems", "Detalles de compras", c => c.PurchaseOrderItems.Any(),
                new[] { "PurchaseOrders", "Books" }),
            new EntityDefinition("PurchaseOrders", "Órdenes de compra", c => c.PurchaseOrders.Any(),
                new[] { "Suppliers" }),
            new EntityDefinition("Reviews", "Reseñas", c => c.Reviews.Any(), new[] { "Books", "Customers" }),
            new EntityDefinition("Roles", "Roles", c => c.Roles.Any(), new string[0]),
            new EntityDefinition("ShoppingCarts", "Carritos", c => c.ShoppingCarts.Any(), new[] { "Customers" }),
            new EntityDefinition("Suppliers", "Proveedores", c => c.Suppliers.Any(), new string[0],
                new SuppliersGenerator().GenerateRecords)
        };

        public List<GenerationStatus> GetStatuses()
        {
            using (var context = new BookStoreDBEntities())
            {
                var presence = Definitions.ToDictionary(d => d.Name,
                    d => d.HasData(context), StringComparer.OrdinalIgnoreCase);
                return Definitions.Select(d => Evaluate(d, presence)).ToList();
            }
        }

        public GenerationResult Generate(string entityName, int count)
        {
            if (count < 1 || count > MaxBatchSize)
            {
                throw new GenerationValidationException(
                    "La cantidad debe estar entre 1 y " + MaxBatchSize + ".");
            }

            var definition = FindDefinition(entityName);

            using (var context = new BookStoreDBEntities())
            using (var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                var presence = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
                presence[definition.Name] = definition.HasData(context);
                foreach (string dependency in definition.Dependencies)
                {
                    presence[dependency] = FindDefinition(dependency).HasData(context);
                }

                var state = Evaluate(definition, presence);
                if (!state.CanGenerate)
                {
                    throw new GenerationValidationException(state.DisplayName + ": " + state.Message);
                }

                int inserted = definition.Write(context, count);
                transaction.Commit();
                return new GenerationResult
                {
                    DisplayName = definition.DisplayName,
                    GeneratedCount = inserted
                };
            }
        }

        private static EntityDefinition FindDefinition(string name)
        {
            var definition = Definitions.FirstOrDefault(d =>
                string.Equals(d.Name, (name ?? "").Trim(), StringComparison.OrdinalIgnoreCase));
            if (definition == null)
            {
                throw new GenerationValidationException("Seleccione una entidad válida.");
            }
            return definition;
        }

        private static GenerationStatus Evaluate(EntityDefinition definition,
            IDictionary<string, bool> presence)
        {
            var state = new GenerationStatus
            {
                EntityName = definition.Name,
                DisplayName = definition.DisplayName,
                HasData = presence[definition.Name],
                IsImplemented = definition.Write != null
            };

            if (state.HasData)
            {
                state.StateLabel = "Con datos";
                state.Message = "La tabla ya contiene datos. La generación está bloqueada.";
                return state;
            }

            var missing = definition.Dependencies.Where(name => !presence[name])
                .Select(name => FindDefinition(name).DisplayName).ToArray();
            if (missing.Length > 0)
            {
                state.StateLabel = "Requiere datos";
                state.Message = "Primero genere datos de: " + string.Join(", ", missing) + ".";
                return state;
            }

            if (!state.IsImplemented)
            {
                state.StateLabel = "Pendiente";
                state.Message = "Generación disponible en un próximo bloque.";
                return state;
            }

            state.CanGenerate = true;
            state.StateLabel = "Disponible";
            state.Message = "Tabla vacía y requisitos satisfechos.";
            return state;
        }
    }
}
