# PAP.BookStore

Caso práctico 1 de Programación Avanzada. Aplicación de librería desarrollada
con ASP.NET MVC 5, .NET Framework 4.8.1 y Entity Framework 6 Database First.

Proyecto en desarrollo.

## Estructura

| Carpeta | Responsabilidad |
| --- | --- |
| `PAP.DataAccess` | Modelo `DataModel.edmx`, entidades y contexto `BookStoreDBEntities`. |
| `PAP.Repositories` | Consultas y persistencia con Entity Framework. |
| `PAP.Business` | Reglas de negocio y validaciones. |
| `PAP.Mvc` | Controladores, vistas y navegación de la aplicación. |
| `database` | Scripts para reconstruir la estructura de la base de datos. |

## Requisitos

- Windows y Visual Studio 2026, utilizado durante el desarrollo.
- Herramientas de desarrollo de ASP.NET y .NET Framework 4.8.1.
- SQL Server LocalDB, instancia `(localdb)\MSSQLLocalDB`.
- Herramientas de Entity Framework Designer para abrir `DataModel.edmx`.

Las versiones de las dependencias están declaradas en los archivos
`packages.config` de cada proyecto. Entity Framework utiliza la versión 6.5.2.

## Preparar la base de datos

En una consulta conectada a la instancia LocalDB, ejecutar completos y en orden:

1. `database/01_CrearBookStoreDB.sql`.
2. `database/02_AmpliarBookStoreDB.sql`.

El primer archivo crea `BookStoreDB` y la tabla `Books` inicial. El segundo
agrega las otras tablas, relaciones y los roles `Admin`, `Manager` y `Cashier`.
En una base nueva, las consultas finales del segundo script deben mostrar
20 tablas y 21 claves foráneas.

Los scripts omiten los objetos que ya existen y no eliminan registros.
No modifican automáticamente objetos existentes con una estructura diferente.
Los archivos permiten reconstruir la estructura; los registros ingresados
manualmente en otra computadora no se transfieren con Git.

## Ejecutar la aplicación

1. Abrir `PAP.BookStore.slnx` en Visual Studio.
2. En **Solution Explorer**, hacer clic derecho sobre la solución y seleccionar
   **Restore NuGet Packages**.
3. Preparar la base de datos con los dos scripts indicados arriba.
4. Verificar que la conexión `BookStoreDBEntities`, en `PAP.Mvc/Web.config`,
   apunte a `(localdb)\MSSQLLocalDB` y a `BookStoreDB`.
5. Hacer clic derecho sobre `PAP.Mvc` y seleccionar **Set as Startup Project**.
6. Seleccionar **Build > Build Solution** y ejecutar con **Ctrl + F5**.

El modelo y sus clases generadas ya están incluidos en el proyecto. Al cambiar
el esquema, abrir `DataModel.edmx` y utilizar **Update Model from Database...**.

## Avance actual

- Base de datos y modelo Database First configurados.
- Categorías: listado, alta, edición, eliminación y detalle en ventanas modales;
  validaciones básicas comprobadas en el navegador.
- Géneros: Repository, Manager y Controller incorporados; vista en preparación.
- Otros módulos, generación masiva y pantalla Admin pendientes.

Este archivo se actualizará conforme se completen los módulos.
