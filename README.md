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

## Base de datos 

Se debe de ejecutar los scripts en orden

1. `database/01_CrearBookStoreDB.sql`.
2. `database/02_AmpliarBookStoreDB.sql`.

El primer archivo crea `BookStoreDB` y la tabla `Books` inicial. El segundo
agrega las otras tablas, relaciones y los roles `Admin`, `Manager` y `Cashier`.
En una base nueva, las consultas finales del segundo script deben mostrar
20 tablas y 21 claves foráneas.

Los scripts omiten los objetos que ya existen y no eliminan registros.

