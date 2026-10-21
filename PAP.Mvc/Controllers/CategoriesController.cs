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
    public class CategoriesController : Controller
    {
        private readonly CategoryManager _manager = new CategoryManager();

        [HttpGet]
        public ActionResult Index()
        {
            return ShowIndex(new Category(), null);
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
                var category = _manager.GetById(id.Value);

                return Json(new
                {
                    category.Id,
                    category.Name,
                    category.Description
                }, JsonRequestBehavior.AllowGet);
            }
            catch (KeyNotFoundException)
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Description")] Category category)
        {
            category = category ?? new Category();

            if (!ModelState.IsValid)
            {
                return ShowIndex(category, "create");
            }

            try
            {
                _manager.Add(category);
                TempData["Success"] = "La categoría se agregó correctamente.";
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
                Trace.TraceError("Error al agregar una categoria: {0}", error);
                ModelState.AddModelError("",
                    "No se pudo guardar la categoría. Actualice la página e inténtelo de nuevo.");
            }

            return ShowIndex(category, "create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] Category category)
        {
            category = category ?? new Category();

            if (!ModelState.IsValid)
            {
                return ShowIndex(category, "edit");
            }

            try
            {
                _manager.Update(category);
                TempData["Success"] = "La categoría se actualizó correctamente.";
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
                Trace.TraceError("Error al editar una categoria: {0}", error);
                ModelState.AddModelError("",
                    "No se pudieron guardar los cambios. Actualice la página e inténtelo de nuevo.");
            }

            return ShowIndex(category, "edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue || id.Value <= 0 || !ModelState.IsValid)
            {
                TempData["Error"] = "Seleccione una categoría válida para eliminar.";
                return RedirectToAction("Index");
            }

            try
            {
                _manager.Delete(id.Value);
                TempData["Success"] = "La categoría se eliminó correctamente.";
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
                Trace.TraceError("Error al eliminar una categoria: {0}", error);
                TempData["Error"] =
                    "No se pudo eliminar la categoría. Actualice la página e inténtelo de nuevo.";
            }

            return RedirectToAction("Index");
        }

        private ActionResult ShowIndex(Category category, string openModal)
        {
            ViewBag.Form = category;
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
