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
    public class SuppliersController : Controller
    {
        private readonly SupplierManager _manager = new SupplierManager();

        [HttpGet]
        public ActionResult Index()
        {
            return ShowIndex(new Supplier { IsActive = true }, null);
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
                var supplier = _manager.GetById(id.Value);

                return Json(new
                {
                    supplier.Id,
                    supplier.Name,
                    supplier.ContactName,
                    supplier.Email,
                    supplier.Phone,
                    supplier.Address,
                    supplier.IsActive
                }, JsonRequestBehavior.AllowGet);
            }
            catch (KeyNotFoundException)
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,ContactName,Email,Phone,Address,IsActive")] Supplier supplier)
        {
            supplier = supplier ?? new Supplier();

            if (!ModelState.IsValid)
            {
                return ShowIndex(supplier, "create");
            }

            try
            {
                _manager.Add(supplier);
                TempData["Success"] = "Proveedor agregado con exito!";
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
                Trace.TraceError("Error al agregar un proveedor: {0}", error);
                ModelState.AddModelError("",
                    "No se pudo guardar el proveedor. Actualice e intente de nuevo...");
            }

            return ShowIndex(supplier, "create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,ContactName,Email,Phone,Address,IsActive")] Supplier supplier)
        {
            supplier = supplier ?? new Supplier();

            if (!ModelState.IsValid)
            {
                return ShowIndex(supplier, "edit");
            }

            try
            {
                _manager.Update(supplier);
                TempData["Success"] = "Proveedor actualizado con exito!";
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
                Trace.TraceError("Error al editar un proveedor: {0}", error);
                ModelState.AddModelError("",
                    "No se pudieron guardar los cambios. Actualice la página e inténtelo de nuevo...");
            }

            return ShowIndex(supplier, "edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue || id.Value <= 0 || !ModelState.IsValid)
            {
                TempData["Error"] = "Seleccione un proveedor válida para eliminar.";
                return RedirectToAction("Index");
            }

            try
            {
                _manager.Delete(id.Value);
                TempData["Success"] = "El proveedor se eliminó correctamente.";
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
                Trace.TraceError("Error al eliminar un proveedor: {0}", error);
                TempData["Error"] =
                    "No se pudo eliminar el proveedor. Actualice la página e inténtelo de nuevo...";
            }

            return RedirectToAction("Index");
        }

        private ActionResult ShowIndex(Supplier supplier, string openModal)
        {
            ViewBag.Form = supplier;
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
