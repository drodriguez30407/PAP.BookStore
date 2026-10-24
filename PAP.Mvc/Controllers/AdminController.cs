using System;
using System.Diagnostics;
using System.Web.Mvc;
using PAP.Generators;

namespace PAP.Mvc.Controllers
{
    public class AdminController : Controller
    {
        private readonly GenerationService _service = new GenerationService();

        [HttpGet]
        public ActionResult Index()
        {
            return View(_service.GetStatuses());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Generate(string entityName, int? count)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(entityName) || !count.HasValue)
            {
                TempData["Error"] = "Seleccione una entidad y una cantidad válidas.";
                return RedirectToAction("Index");
            }

            try
            {
                var result = _service.Generate(entityName, count.Value);
                TempData["Success"] = "Se generaron " + result.GeneratedCount
                    + " registros en " + result.DisplayName + ".";
            }
            catch (GenerationValidationException error)
            {
                TempData["Error"] = error.Message;
            }
            catch (Exception error)
            {
                Trace.TraceError("Error al generar datos de {0}: {1}", entityName, error);
                TempData["Error"] =
                    "No se pudieron generar los datos. Actualice la página e inténtelo de nuevo.";
            }

            return RedirectToAction("Index");
        }
    }
}
