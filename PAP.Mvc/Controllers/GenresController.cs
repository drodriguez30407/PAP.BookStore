using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Net;
using System.Web.Mvc;
using PAP.Business;
using PAP.DataAccess;

namespace PAP.Mvc.Controllers
{
    public class GenresController : Controller
    {
        private readonly GenreManager _manager = new GenreManager();

        [HttpGet]
        public ActionResult Index()
        {
            return ShowIndex(new Genre(), null);
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
                var genre = _manager.GetById(id.Value);

                return Json(new
                {
                    genre.Id,
                    genre.Name,
                    genre.Description
                }, JsonRequestBehavior.AllowGet);
            }
            catch (KeyNotFoundException)
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Description")] Genre genre)
        {
            genre = genre ?? new Genre();

            if (!ModelState.IsValid)
            {
                return ShowIndex(genre, "create");
            }

            try
            {
                _manager.Add(genre);
                TempData["Success"] = "El género se agregó correctamente.";
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
                Trace.TraceError("Error al agregar un género: {0}", error);
                ModelState.AddModelError("",
                    "No se pudo guardar el género. Actualice e inténtelo de nuevo.");
            }

            return ShowIndex(genre, "create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] Genre genre)
        {
            genre = genre ?? new Genre();

            if (!ModelState.IsValid)
            {
                return ShowIndex(genre, "edit");
            }

            try
            {
                _manager.Update(genre);
                TempData["Success"] = "El género se actualizó correctamente.";
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
                Trace.TraceError("Error al editar un género: {0}", error);
                ModelState.AddModelError("",
                    "No se pudieron guardar los cambios. Actualice e inténtelo de nuevo...");
            }

            return ShowIndex(genre, "edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue || id.Value <= 0 || !ModelState.IsValid)
            {
                TempData["Error"] = "Seleccione un género válido para eliminar.";
                return RedirectToAction("Index");
            }

            try
            {
                _manager.Delete(id.Value);
                TempData["Success"] = "El género se eliminó correctamente.";
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
                Trace.TraceError("Error al eliminar un género: {0}", error);
                TempData["Error"] =
                    "No se pudo eliminar el género. Actualice e inténtelo de nuevo...";
            }

            return RedirectToAction("Index");
        }

        private ActionResult ShowIndex(Genre genre, string openModal)
        {
            ViewBag.Form = genre;
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
