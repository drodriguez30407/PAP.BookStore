using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Web.Mvc;
using PAP.Business;
using PAP.DataAccess;

namespace PAP.Mvc.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookManager _manager = new BookManager();

        [HttpGet]
        public ActionResult Index()
        {
            return ShowIndex(new Book(), null);
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (!id.HasValue || id.Value <= 0 || !ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var book = _manager.GetById(id.Value);

                return Json(new
                {
                    book.Id,
                    book.Title,
                    book.Description,
                    PublicationDate = book.PublicationDate.HasValue
                        ? book.PublicationDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null,
                    Price = book.Price.ToString("0.00", CultureInfo.InvariantCulture),
                    book.CategoryId,
                    CategoryName = book.Category == null ? "Sin categoría" : book.Category.Name,
                    book.GenreId,
                    GenreName = book.Genre == null ? "Sin género" : book.Genre.Name,
                    book.PublisherId,
                    PublisherName = book.Publisher == null ? "Sin editorial" : book.Publisher.Name,
                    book.ISBN,
                    book.Pages,
                    book.Language,
                    book.CoverImageUrl,
                    CreatedAt = book.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                    UpdatedAt = book.UpdatedAt.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                }, JsonRequestBehavior.AllowGet);
            }
            catch (KeyNotFoundException)
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Description,PublicationDate,CategoryId,GenreId,PublisherId,ISBN,Pages,Language,CoverImageUrl")] Book book)
        {
            book = book ?? new Book();
            ReadPrice(book);

            if (!ModelState.IsValid)
            {
                return ShowIndex(book, "create");
            }

            try
            {
                _manager.Add(book);
                TempData["Success"] = "El libro se agregó correctamente.";
                return RedirectToAction("Index");
            }
            catch (ArgumentException error)
            {
                ModelState.AddModelError("", error.Message);
            }
            catch (InvalidOperationException error)
            {
                ModelState.AddModelError("", error.Message);
            }
            catch (DbUpdateException error)
            {
                Trace.TraceError("Error al agregar un libro: {0}", error);
                ModelState.AddModelError("",
                    "No se pudo guardar el libro. Actualice la página e inténtelo de nuevo.");
            }

            return ShowIndex(book, "create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,PublicationDate,CategoryId,GenreId,PublisherId,ISBN,Pages,Language,CoverImageUrl")] Book book)
        {
            book = book ?? new Book();
            ReadPrice(book);

            if (!ModelState.IsValid)
            {
                return ShowIndex(book, "edit");
            }

            try
            {
                _manager.Update(book);
                TempData["Success"] = "El libro se actualizó correctamente.";
                return RedirectToAction("Index");
            }
            catch (ArgumentException error)
            {
                ModelState.AddModelError("", error.Message);
            }
            catch (InvalidOperationException error)
            {
                ModelState.AddModelError("", error.Message);
            }
            catch (KeyNotFoundException)
            {
                return HttpNotFound();
            }
            catch (DbUpdateException error)
            {
                Trace.TraceError("Error al editar un libro: {0}", error);
                ModelState.AddModelError("",
                    "No se pudieron guardar los cambios. Actualice la página e inténtelo de nuevo.");
            }

            return ShowIndex(book, "edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue || id.Value <= 0 || !ModelState.IsValid)
            {
                TempData["Error"] = "Seleccione un libro válido para eliminar.";
                return RedirectToAction("Index");
            }

            try
            {
                _manager.Delete(id.Value);
                TempData["Success"] = "El libro se eliminó correctamente.";
            }
            catch (ArgumentException error)
            {
                TempData["Error"] = error.Message;
            }
            catch (InvalidOperationException error)
            {
                TempData["Error"] = error.Message;
            }
            catch (KeyNotFoundException error)
            {
                TempData["Error"] = error.Message;
            }
            catch (DbUpdateException error)
            {
                Trace.TraceError("Error al eliminar un libro: {0}", error);
                TempData["Error"] =
                    "No se pudo eliminar el libro. Actualice la página e inténtelo de nuevo.";
            }

            return RedirectToAction("Index");
        }

        private ActionResult ShowIndex(Book book, string openModal)
        {
            ViewBag.Form = book;
            ViewBag.OpenModal = openModal;
            ViewBag.CategoryOptions = new SelectList(_manager.GetCategories(), "Id", "Name", book.CategoryId);
            ViewBag.GenreOptions = new SelectList(_manager.GetGenres(), "Id", "Name", book.GenreId);
            ViewBag.PublisherOptions = new SelectList(_manager.GetPublishers(), "Id", "Name", book.PublisherId);
            return View("Index", _manager.GetAll());
        }

        // HTML number inputs submit a dot as decimal separator, regardless of server culture.
        // Price is deliberately excluded from Bind and parsed only here.
        private void ReadPrice(Book book)
        {
            var value = ValueProvider.GetValue("Price");
            if (value != null) ModelState.SetModelValue("Price", value);
            if (value == null || string.IsNullOrWhiteSpace(value.AttemptedValue))
            {
                ModelState.AddModelError("Price", "El precio es obligatorio.");
                return;
            }
            decimal price;
            if (!decimal.TryParse(value.AttemptedValue.Trim(),
                NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture, out price))
            {
                ModelState.AddModelError("Price", "Ingrese un precio válido, sin separadores de miles.");
                return;
            }
            book.Price = price;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _manager.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
