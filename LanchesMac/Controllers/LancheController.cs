using LanchesMac.Repositories.Interfaces;
using LanchesMac.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LanchesMac.Controllers
{
    public class LancheController : Controller
    {
        private readonly ILanchesRepository _lanchesRepository;

        public LancheController(ILanchesRepository lanchesRepository)
        {
            _lanchesRepository = lanchesRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
            //ViewData["Titulo"] = "Todos os Lanches"; // o ViewData armazena como se fosse um dicionário
            //ViewData["Data"] = DateTime.Now;

            //var lanches = _lanchesRepository.Lanches;
            //var totalLanches = lanches.Count();

            //ViewBag.Total = "Total de lanches: "; // já o ViewBag armazena como se fosse uma classe e suas propriedades
            //ViewBag.TotalLanches = totalLanches;

            var lancheListViewModel = new LancheListViewModel();
            lancheListViewModel.Lanches = _lanchesRepository.Lanches;
            lancheListViewModel.CategoriaAtual = "Categoria Atual";

            return View(lancheListViewModel);
        }
    }
}
