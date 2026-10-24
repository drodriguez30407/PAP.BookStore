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
    public class PublishersController : Controller
    {
        private readonly PublisherManager _manager = new PublisherManager();

        [HttpGet]
        public ActionResult Index()
        {
            return ShowIndex(new Publisher(), null);
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
                var publisher = _manager.GetById(id.Value);

                return Json(new
                {
                    publisher.Id,
                    publisher.Name,
                    publisher.Country,
                    publisher.Website,
                    publisher.Email,
                    CreatedAt = publisher.CreatedAt.ToString(
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
        public ActionResult Create([Bind(Include = "Name,Country,Website,Email")] Publisher publisher)
        {
            publisher = publisher ?? new Publisher();

            if (!ModelState.IsValid)
            {
                return ShowIndex(publisher, "create");
            }

            try
            {
                _manager.Add(publisher);
                TempData["Success"] = "La editorial se agregó correctamente.";
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
                Trace.TraceError("Error al agregar una editorial: {0}", error);
                ModelState.AddModelError("",
                    "No se pudo guardar la editorial. Actualice la página e inténtelo de nuevo.");
            }

            return ShowIndex(publisher, "create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Country,Website,Email")] Publisher publisher)
        {
            publisher = publisher ?? new Publisher();

            if (!ModelState.IsValid)
            {
                return ShowIndex(publisher, "edit");
            }

            try
            {
                _manager.Update(publisher);
                TempData["Success"] = "La editorial se actualizó correctamente.";
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
                Trace.TraceError("Error al editar una editorial: {0}", error);
                ModelState.AddModelError("",
                    "No se pudieron guardar los cambios. Actualice la página e inténtelo de nuevo.");
            }

            return ShowIndex(publisher, "edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue || id.Value <= 0 || !ModelState.IsValid)
            {
                TempData["Error"] = "Seleccione una editorial válida para eliminar.";
                return RedirectToAction("Index");
            }

            try
            {
                _manager.Delete(id.Value);
                TempData["Success"] = "La editorial se eliminó correctamente.";
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
                Trace.TraceError("Error al eliminar una editorial: {0}", error);
                TempData["Error"] =
                    "No se pudo eliminar la editorial. Actualice la página e inténtelo de nuevo.";
            }

            return RedirectToAction("Index");
        }

        private ActionResult ShowIndex(Publisher publisher, string openModal)
        {
            ViewBag.Form = publisher;
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
