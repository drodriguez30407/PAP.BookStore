# PAP.BookStore

Caso práctico 1 de Programación Avanzada - Universidad Fidelitas

Aplicación de biblioteca 

Tecnologias usadas: 
ASP.NET MVC 5, .NET Framework 4.8.1 y Entity Framework 6 Database First


## Estructura

| Carpeta | Responsabilidad |
| --- | --- |
| `PAP.DataAccess` | Modelo `DataModel.edmx`, entidades y contexto `BookStoreDBEntities`. |
| `PAP.Repositories` | Consultas y persistencia con Entity Framework. |
| `PAP.Business` | Reglas de negocio y validaciones. |
| `PAP.Mvc` | Controladores, vistas y navegación de la aplicación. |
| `database` | Scripts para reconstruir la estructura de la base de datos. |

## Base de datos en carpeta database
Se ejecutan en orden menor a mayor, si ya existe DB se omiten

