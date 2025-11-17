using LanchesMac.Areas.Admin.Servicos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanchesMac.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminGraficoController : Controller
    {
        private readonly GraficoVendasService _graficoVendasService;

        public AdminGraficoController(GraficoVendasService graficoVendasService)
        {
            _graficoVendasService = graficoVendasService;
        }

        public JsonResult VendasLanches(int dias)
        {
            var lanches = _graficoVendasService.GetVendasLanches(dias);
            return Json(lanches);
        }

        public IActionResult Index(int dias)
        {
            return View();
        }

        public IActionResult VendasMensal(int dias)
        {
            return View();
        }

        public IActionResult VendasSemanal(int dias)
        {
            return View();
        }   
    }
}
