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
    public class CustomersController : Controller
    {
        private readonly CustomerManager _manager = new CustomerManager();

        [HttpGet]
        public ActionResult Index()
        {
            return ShowIndex(new Customer { IsActive = true }, null);
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
                var customer = _manager.GetById(id.Value);

                return Json(new
                {
                    customer.Id,
                    customer.FirstName,
                    customer.LastName,
                    customer.Email,
                    customer.Phone,
                    BirthDate = customer.BirthDate.HasValue
                        ? customer.BirthDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null,
                    customer.IsActive,
                    CreatedAt = customer.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                    UpdatedAt = customer.UpdatedAt.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                }, JsonRequestBehavior.AllowGet);
            }
            catch (KeyNotFoundException)
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FirstName,LastName,Email,Phone,BirthDate,IsActive")] Customer customer, string password, string confirmPassword)
        {
            customer = customer ?? new Customer();

            if (!ModelState.IsValid)
            {
                return ShowIndex(customer, "create");
            }

            try
            {
                _manager.Add(customer, password, confirmPassword);
                TempData["Success"] = "El cliente se agregó correctamente.";
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
                Trace.TraceError("Error al agregar un cliente: {0}", error);
                ModelState.AddModelError("",
                    "No se pudo guardar el cliente. Actualice la página e inténtelo de nuevo...");
            }

            return ShowIndex(customer, "create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Email,Phone,BirthDate,IsActive")] Customer customer, string password, string confirmPassword)
        {
            customer = customer ?? new Customer();

            if (!ModelState.IsValid)
            {
                return ShowIndex(customer, "edit");
            }

            try
            {
                _manager.Update(customer, password, confirmPassword);
                TempData["Success"] = "El cliente se actualizó correctamente.";
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
                Trace.TraceError("Error al editar un cliente: {0}", error);
                ModelState.AddModelError("",
                    "No se pudieron guardar los cambios. Actualice la página e inténtelo de nuevo...");
            }

            return ShowIndex(customer, "edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue || id.Value <= 0 || !ModelState.IsValid)
            {
                TempData["Error"] = "Seleccione un cliente válido para eliminar.";
                return RedirectToAction("Index");
            }

            try
            {
                _manager.Delete(id.Value);
                TempData["Success"] = "El cliente se eliminó correctamente.";
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
                Trace.TraceError("Error al eliminar un cliente: {0}", error);
                TempData["Error"] =
                    "No se pudo eliminar el cliente. Actualice la página e inténtelo de nuevo...";
            }

            return RedirectToAction("Index");
        }

        private ActionResult ShowIndex(Customer customer, string openModal)
        {
            ModelState passwordState;
            if (ModelState.TryGetValue("password", out passwordState)) passwordState.Value = null;
            if (ModelState.TryGetValue("confirmPassword", out passwordState)) passwordState.Value = null;
            ViewBag.Form = customer;
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
