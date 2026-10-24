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
    public class AuthorsController : Controller
    {
        private readonly AuthorManager _manager = new AuthorManager();

        [HttpGet]
        public ActionResult Index()
        {
            return ShowIndex(new Author(), null);
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
                var author = _manager.GetById(id.Value);

                return Json(new
                {
                    author.Id,
                    author.FirstName,
                    author.LastName,
                    author.Biography,
                    BirthDate = author.BirthDate.HasValue
                        ? author.BirthDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                        : null,
                    author.Country,
                    CreatedAt = author.CreatedAt.ToString(
                        "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                }, JsonRequestBehavior.AllowGet);
            }
            catch (KeyNotFoundException)
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "FirstName,LastName,Biography,BirthDate,Country")] Author author)
        {
            author = author ?? new Author();

            if (!ModelState.IsValid)
            {
                return ShowIndex(author, "create");
            }

            try
            {
                _manager.Add(author);
                TempData["Success"] = "El autor se agregó correctamente.";
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
                Trace.TraceError("Error al agregar un autor: {0}", error);
                ModelState.AddModelError("",
                    "No se pudo guardar el autor. Actualice e inténtelo de nuevo...");
            }

            return ShowIndex(author, "create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "Id,FirstName,LastName,Biography,BirthDate,Country")] Author author)
        {
            author = author ?? new Author();

            if (!ModelState.IsValid)
            {
                return ShowIndex(author, "edit");
            }

            try
            {
                _manager.Update(author);
                TempData["Success"] = "El autor se actualizó correctamente.";
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
                Trace.TraceError("Error al editar un autor: {0}", error);
                ModelState.AddModelError("",
                    "No se pudieron guardar los cambios. Actualice e inténtelo de nuevo...");
            }

            return ShowIndex(author, "edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue || id.Value <= 0 || !ModelState.IsValid)
            {
                TempData["Error"] = "Seleccione un autor válido para eliminar.";
                return RedirectToAction("Index");
            }

            try
            {
                _manager.Delete(id.Value);
                TempData["Success"] = "El autor se eliminó correctamente.";
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
                Trace.TraceError("Error al eliminar un autor: {0}", error);
                TempData["Error"] =
                    "No se pudo eliminar el autor. Actualice e inténtelo de nuevo...";
            }

            return RedirectToAction("Index");
        }

        private ActionResult ShowIndex(Author author, string openModal)
        {
            ViewBag.Form = author;
            ViewBag.OpenModal = openModal;
            return View("Index", _manager.GetAll());
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
